using System.Buffers;
using System;
using Bedrock.Framework.Protocols.Kafka.Utils;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public struct KafkaResponse : IDisposable
    {
        public KafkaResponse(MemoryBufferWriter<byte> responseMessageWriter, int correlationId)
        {
            Writer = responseMessageWriter;
            CorrelationId = correlationId;
        }

        public readonly int CorrelationId;

        public readonly ReadOnlySequence<byte> ResponseMessage => Writer.AsReadOnlySequence;

        private readonly MemoryBufferWriter<byte> Writer;

        public void Dispose()
        {
            (Writer as IDisposable)?.Dispose();
        }
    }
}
