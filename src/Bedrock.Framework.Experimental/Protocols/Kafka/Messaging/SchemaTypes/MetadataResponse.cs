namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct MetadataResponse
    {
        public int ThrottleTimeMs;

        public Broker[] Brokers;

        //.... other properties
    }
}
