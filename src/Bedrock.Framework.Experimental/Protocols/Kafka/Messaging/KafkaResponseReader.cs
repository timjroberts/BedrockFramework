using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public class KafkaResponseReader : IMessageReader<KafkaResponse>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out KafkaResponse message)
        {
            var reader = new SequenceReader<byte>(input);

            if (!reader.TryReadInt(out int size) || input.Length < size + sizeof(int))
            {
                message = default;

                return false;
            }

            // Could this still fail even though we've ensured that the sequence should contains
            // the bytes we need
            reader.TryReadShort(out short correlationId);

            // Adjust size to remove the bytes occupied by the correlationId
            size -= sizeof(short);

            var memory = MemoryPool<byte>.Shared.Rent(size);
            var messageSpan = input.Slice(reader.Position, size);

            messageSpan.CopyTo(memory.Memory.Span);

            message = new KafkaResponse()
            {
                CorrelationId = correlationId,
                ResponseMessage = memory
            };

            consumed = examined = messageSpan.End;

            return true;
        }
    }
}
