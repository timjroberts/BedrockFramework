using System;
using System.Buffers;

namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct KafkaRequest
    {
        public int CorrelationId;

        public string ClientId;

        public ApiKey ApiKey;

        public short ApiVersion;

        public IKafkaMessageWriter RequestWriter;

        public object RequestMessage;
    }
}
