using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    [KakfaMessage(ApiKey.ApiVersions)]
    public class ApiVersionsMessage : KafkaMessageDescriptor<Empty, ApiVersionsResponse>
    { }
}
