namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public interface IKafkaMessageDescriptor
    {

    }

    /// <summary>
    /// Describes an API message that can be used as part of the wire protocol between a Kafka client and broker.
    /// </summary>
    /// <typeparam name="TRequestSchema">The request schema type.</typeparam>
    /// <typeparam name="TResponseSchema">The response schema type.</typeparam>
    /// <remarks>
    /// A message descriptor brings together a request and response schema type for a version of a Kafka API
    /// message that is implemented by a Kafka broker. The request and response schema types are inclusive of all
    /// prior versions of the message up to and including the highest version supported by this protocol implementation.
    /// 
    /// The highest version of a message supported by both Kafka client and broker should be used.
    /// </remarks>
    public abstract class KafkaMessageDescriptor<TRequestSchema, TResponseSchema> : IKafkaMessageDescriptor
    {

    }
}
