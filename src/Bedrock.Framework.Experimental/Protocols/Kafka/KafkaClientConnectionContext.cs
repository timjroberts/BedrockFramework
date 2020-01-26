using System.Buffers;
using System.Diagnostics;
using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;
using Microsoft.AspNetCore.Connections;
using Bedrock.Framework.Protocols.Kafka.Messaging;

namespace Bedrock.Framework.Protocols.Kafka
{
    /// <summary>
    /// Encapsulates a client Kafka connection.
    /// </summary>
    public interface IKafkaClientConnectionContext
    {
        /// <summary>
        /// Starts the client connection.
        /// </summary>
        /// <param name="connectionContext">The underlying connection.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the start operation.</param>
        /// <returns>A <see cref="Task"/> that will complete once the client connection has started.</returns>
        Task StartAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Serializes and sends a request through the underlying connection and waits for the response.
        /// </summary>
        /// <typeparam name="TRequestSchema">The request schema type.</typeparam>
        /// <typeparam name="TResponseSchema">The response schema tyoe.</typeparam>
        /// <param name="request">The request to send.</param>
        /// <param name="messageDescriptor">Provides context for how the request and response should be written and
        /// read from the underlying connection.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the send operation.</param>
        /// <returns>A <see cref="Task"/> that will complete with the response for <paramref name="request"/>. If the
        /// send operation results in an exception then the Task will complete with the exception.</returns>
        Task<TResponseSchema> SendAsync<TRequestSchema, TResponseSchema>(
            TRequestSchema request,
            IKafkaMessageDescriptor messageDescriptor,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Encapsulates a client Kafka connection that channels requests to the underlying connection
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

        public KafkaClientConnectionContext(IMessageWriter<KafkaRequest> requestWriter, IMessageReader<KafkaResponse> responseReader)
        {
            _requestWriter = requestWriter;
            _responseReader = responseReader;
        }

        public Task StartAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default)
        {
            _ = WriteRequestsAsync(connectionContext, cancellationToken);
            _ = ReadResponsesAsync(connectionContext, cancellationToken);

            return Task.CompletedTask;
        }

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
                    var correlationId = ++_correlationId;

                    _requestQueue.Enqueue((correlationId, request.messageDescriptor, request.tcs));

                    await writer.WriteAsync(
                        _requestWriter,
                        new KafkaRequest()
                        {
                            CorrelationId = correlationId,
                            ClientId = string.Empty,
                            ApiKey = request.messageDescriptor.ApiKey,
                            ApiVersion = request.messageDescriptor.ApiVersion,
                            RequestWriter = request.messageDescriptor.GetRequestWriter(),
                            RequestMessage = request.request
                        },
                        cancellationToken);
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

                var hasRequestInFlight = _requestQueue.TryDequeue(
                    out (int correlationId, IKafkaMessageDescriptor messageDescriptor, TaskCompletionSource<object> tcs) inflightRequestContext);

                Debug.Assert(hasRequestInFlight, "Protocol Error: received unexpected message from server.");

                var response = result.Message;

                try
                {
                    if (response.CorrelationId != inflightRequestContext.correlationId)
                    {
                        inflightRequestContext.tcs.SetException(new Exception("Protocol Exception: correlation ID mis-match."));

                        continue;
                    }

                    var responseReader = inflightRequestContext.messageDescriptor.GetResponseReader();

                    inflightRequestContext.tcs.SetResult(responseReader.ReadMessage(new ReadOnlySequence<byte>(response.ResponseMessage.Memory)));
                }
                finally
                {
                    response.ResponseMessage.Dispose();
                    reader.Advance();
                }
            }
        }
    }
}
