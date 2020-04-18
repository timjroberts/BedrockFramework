using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;
using Bedrock.Framework.Protocols.Kafka.Utils;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public readonly struct KafkaRequest : IDisposable
    {
        public KafkaRequest(MemoryBufferWriter<byte> requestMessageWriter, ApiKey apiKey, short apiVersion, int correlationId, string clientId = default)
        {
            Writer = requestMessageWriter;
            ApiKey = apiKey;
            ApiVersion = apiVersion;
            CorrelationId = correlationId;
            ClientId = clientId;
        }

        public readonly ApiKey ApiKey;

        public readonly short ApiVersion;

        public readonly int CorrelationId;

        public readonly string ClientId;

        public readonly ReadOnlySequence<byte> RequestMessage => Writer.AsReadOnlySequence;

        private readonly MemoryBufferWriter<byte> Writer;

        public void Dispose()
        {
            (Writer as IDisposable)?.Dispose();
        }
    }
}
