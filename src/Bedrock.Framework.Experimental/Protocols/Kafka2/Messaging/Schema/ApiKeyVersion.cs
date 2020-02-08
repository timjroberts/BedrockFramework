namespace Bedrock.Framework.Protocols.Kafka2.Messaging.SchemaTypes
{
    public struct ApiKeyVersion
    {
        public static short Earliest = short.MinValue;
        public static short Latest = -1;

        public ApiKey ApiKey;

        public short MinVersion;

        public short MaxVersion;
    }
}
