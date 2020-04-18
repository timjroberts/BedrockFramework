using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    /// <summary>
    /// Defines an API message that can be used as part of the wire protocol between a Kafka client and broker.
    /// </summary>
    public interface IKafkaMessageDescriptor
    {
        /// <summary>
        /// Gets the API key that this message represents.
        /// </summary>
        /// <value>An <see cref="ApiKey"/> value.</value>
        ApiKey ApiKey { get; }

        /// <summary>
        /// Gets the API version that this message represents.
        /// </summary>
        /// <value>The numeric version of the API.</value>
        short ApiVersion { get; }

        /// <summary>
        /// Retrieves a writer that can write the request associated with this message.
        /// </summary>
        /// <returns>An <see cref="IKafkaMessageWriter"/>.</returns>
        IKafkaMessageWriter GetRequestWriter();

        /// <summary>
        /// Retrieves a reader that can read the request associated with this message.
        /// </summary>
        /// <returns>An <see cref="IKafkaMessageReader"/>.</returns>
        IKafkaMessageReader GetRequestReader();

        /// <summary>
        /// Retrieves a writer that can write the response associated with this message.
        /// </summary>
        /// <returns>An <see cref="IKafkaMessageWriter"/>.</returns>
        IKafkaMessageWriter GetResponseWriter();

        /// <summary>
        /// Retrieves a reader that can read the response associated with this message.
        /// </summary>
        /// <returns>An <see cref="IKafkaMessageReader"/>.</returns>
        IKafkaMessageReader GetResponseReader();
    }
}
