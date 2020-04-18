namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct Broker
    {
        public int NodeId;

        public string Host;

        public int Port;

        public string Rack;
    }
}
