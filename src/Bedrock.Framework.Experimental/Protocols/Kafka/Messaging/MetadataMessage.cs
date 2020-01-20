using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    [KakfaMessage(ApiKey.Metadata, Version = 9)]
    public class MetadataMessage : KafkaMessageDescriptor<MetadataRequest, MetadataResponse>
    { }
}
