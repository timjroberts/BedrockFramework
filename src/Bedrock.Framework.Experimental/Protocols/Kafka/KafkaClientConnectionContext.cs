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
        private readonly Channel<RequestContext<object>> _requestChannel =
            Channel.CreateUnbounded<RequestContext<object>>(
                new UnboundedChannelOptions()
                {
                    SingleReader = true,
                    SingleWriter = false
                });

        private readonly ConcurrentQueue<RequestContext<int>> _requestQueue = new ConcurrentQueue<RequestContext<int>>();

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

            _requestChannel.Writer.TryWrite(new RequestContext<object>(request, messageDescriptor, tcs));

            return (TResponseSchema)await tcs.Task;
        }

        private async Task WriteRequestsAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default)
        {
            var writer = connectionContext.CreateWriter();

            await foreach (RequestContext<object> request in _requestChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var bufferWriter = new MemoryBufferWriter<byte>();
                var correlationId = ++_correlationId;
                var requestWriter = request.MessageDescriptor.GetRequestWriter();

                _requestQueue.Enqueue(request.WithNewContext(correlationId));

                requestWriter.WriteMessage(request, bufferWriter);

                using var kafkaRequest = new KafkaRequest(bufferWriter, request.MessageDescriptor.ApiKey, request.MessageDescriptor.ApiVersion, correlationId);

                await writer.WriteAsync(_requestWriter, kafkaRequest, cancellationToken);
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

                var hasRequestInFlight = _requestQueue.TryDequeue(out RequestContext<int> requestContext);

                Debug.Assert(hasRequestInFlight, "Protocol Error: received unexpected message from server.");

                try
                {
                    if (kafkaResponse.CorrelationId != requestContext.Item)
                    {
                        requestContext.ResponseCompletionSource.SetException(new Exception("Protocol Exception: correlation ID mis-match."));

                        continue;
                    }

                    var responseReader = requestContext.MessageDescriptor.GetResponseReader();

                    requestContext.ResponseCompletionSource.SetResult(responseReader.ReadMessage(kafkaResponse.ResponseMessage));
                }
                finally
                {
                    reader.Advance();
                }
            }
        }

        private readonly struct RequestContext<T>
        {
            public RequestContext(T item, IKafkaMessageDescriptor messageDescriptor, TaskCompletionSource<object> responseCompletionSource)
            {
                Item = item;
                MessageDescriptor = messageDescriptor;
                ResponseCompletionSource = responseCompletionSource;
            }

            public readonly T Item;

            public readonly IKafkaMessageDescriptor MessageDescriptor;

            public readonly TaskCompletionSource<object> ResponseCompletionSource;

            public RequestContext<TOut> WithNewContext<TOut>(TOut item)
            {
                return new RequestContext<TOut>(item, this.MessageDescriptor, this.ResponseCompletionSource);
            }
        }
    }
}
