namespace Bedrock.Framework.Protocols.Kafka2.Messaging.SchemaTypes
{
    public struct ApiVersionsResponse
    {
        public short ErrorCode;

        public ApiKeyVersion[] ApiKeys;
    }
}
