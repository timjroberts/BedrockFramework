using System.Buffers;
using System;

namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct KafkaResponse
    {
        public short CorrelationId;

        public IMemoryOwner<byte> ResponseMessage;
    }
}
