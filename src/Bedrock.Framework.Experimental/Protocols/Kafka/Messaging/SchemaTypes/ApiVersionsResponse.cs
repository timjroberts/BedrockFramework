namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct ApiVersionsResponse
    {
        public short ErrorCode;

        public ApiKeyVersion[] ApiKeys;
    }
}
