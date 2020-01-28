using System.Threading;
using System.Threading.Tasks;
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
}
