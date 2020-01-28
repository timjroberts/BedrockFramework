using System.Diagnostics;
using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Bedrock.Framework.Protocols.Kafka.Messaging;
using Bedrock.Framework.Protocols.Kafka.Utils;

namespace Bedrock.Framework.Protocols.Kafka
{
    /// <summary>
    /// Encapsulates a client Kafka connection that channels requests to an underlying connection
    /// in the order that they were written; and reads responses in the same order.
    /// </summary>
    public class KafkaClientConnectionContext : IKafkaClientConnectionContext
    {
        private readonly Channel<(object, IKafkaMessageDescriptor, TaskCompletionSource<object>)> _requestChannel =
            Channel.CreateUnbounded<(object, IKafkaMessageDescriptor, TaskCompletionSource<object>)>(
                new UnboundedChannelOptions()
                {
                    SingleReader = true,
                    SingleWriter = false
                });

        private readonly ConcurrentQueue<(int, IKafkaMessageDescriptor, TaskCompletionSource<object>)> _requestQueue =
            new ConcurrentQueue<(int, IKafkaMessageDescriptor, TaskCompletionSource<object>)>();

        private readonly IMessageWriter<KafkaRequest> _requestWriter;
        private readonly IMessageReader<KafkaResponse> _responseReader;

        private int _correlationId = 0;

        /// <summary>
        /// Initializes a new <see cref="KafkaClientConnectionContext"/>.
        /// </summary>
        /// <param name="requestWriter">An <see cref="IMessageWriter{KafkaRequest}"/> that can write Kafka requests to
        /// a buffer.</param>
        /// <param name="responseReader">An <see cref="IMessageReader{KafkaResponse}"/> that can read Kafka responses from
        /// a buffer.</param>
        public KafkaClientConnectionContext(IMessageWriter<KafkaRequest> requestWriter, IMessageReader<KafkaResponse> responseReader)
        {
            _requestWriter = requestWriter;
            _responseReader = responseReader;
        }

        /// <inheritdoc/>
        public Task StartAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default)
        {
            _ = WriteRequestsAsync(connectionContext, cancellationToken);
            _ = ReadResponsesAsync(connectionContext, cancellationToken);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<TResponseSchema> SendAsync<TRequestSchema, TResponseSchema>(
            TRequestSchema request,
            IKafkaMessageDescriptor messageDescriptor,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<object>();

            _requestChannel.Writer.TryWrite((request, messageDescriptor, tcs));

            return (TResponseSchema)await tcs.Task;
        }

        private async Task WriteRequestsAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default)
        {
            var writer = connectionContext.CreateWriter();

            while (await _requestChannel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_requestChannel.Reader.TryRead(out (object request, IKafkaMessageDescriptor messageDescriptor, TaskCompletionSource<object> tcs) request))
                {
                    var bufferWriter = new MemoryBufferWriter<byte>();
                    var correlationId = ++_correlationId;
                    var requestWriter = request.messageDescriptor.GetRequestWriter();

                    _requestQueue.Enqueue((correlationId, request.messageDescriptor, request.tcs));

                    requestWriter.WriteMessage(request, bufferWriter);

                    using var kafkaRequest = new KafkaRequest(bufferWriter, request.messageDescriptor.ApiKey, request.messageDescriptor.ApiVersion, correlationId);

                    await writer.WriteAsync(_requestWriter, kafkaRequest, cancellationToken);
                }
            }
        }

        private async Task ReadResponsesAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default)
        {
            var reader = connectionContext.CreateReader();

            while (true)
            {
                var result = await reader.ReadAsync(_responseReader, cancellationToken);

                if (result.IsCompleted)
                    break;

                using var kafkaResponse = result.Message;

                var hasRequestInFlight = _requestQueue.TryDequeue(
                    out (int correlationId, IKafkaMessageDescriptor messageDescriptor, TaskCompletionSource<object> tcs) inflightRequestContext);

                Debug.Assert(hasRequestInFlight, "Protocol Error: received unexpected message from server.");

                try
                {
                    if (kafkaResponse.CorrelationId != inflightRequestContext.correlationId)
                    {
                        inflightRequestContext.tcs.SetException(new Exception("Protocol Exception: correlation ID mis-match."));

                        continue;
                    }

                    var responseReader = inflightRequestContext.messageDescriptor.GetResponseReader();

                    inflightRequestContext.tcs.SetResult(responseReader.ReadMessage(kafkaResponse.ResponseMessage));
                }
                finally
                {
                    reader.Advance();
                }
            }
        }
    }
}
