using System;

namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct KafkaResponse
    {
        public int Size;

        public short CorrelationId;

        public Memory<byte> ResponseMessage;
    }
}
