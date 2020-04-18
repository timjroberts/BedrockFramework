namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct ApiKeyVersion
    {
        public ApiKey ApiKey;

        public short MinVersion;

        public short MaxVersion;
    }
}
