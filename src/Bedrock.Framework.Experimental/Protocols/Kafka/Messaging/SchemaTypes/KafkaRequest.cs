using System;

namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct KafkaRequest
    {
        public int Size;

        public ApiKey ApiKey;

        public short ApiVersion;

        public short CorrelationId;

        public string ClientId;

        public Memory<byte> RequestMessage;
    }
}
