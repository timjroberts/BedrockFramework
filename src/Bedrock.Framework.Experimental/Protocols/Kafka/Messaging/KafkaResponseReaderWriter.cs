using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Utils;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public class KafkaResponseReaderWriter : IMessageWriter<KafkaResponse>, IMessageReader<KafkaResponse>
    {
        public void WriteMessage(KafkaResponse message, IBufferWriter<byte> output)
        {
            // This will require implementation when the server protocol is implemented
            throw new NotImplementedException();
        }
        
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

            var bufferWriter = new MemoryBufferWriter<byte>();
            var messageSpan = input.Slice(reader.Position, size);

            messageSpan.CopyTo(bufferWriter.GetSpan(size));

            message = new KafkaResponse(bufferWriter, correlationId);

            consumed = examined = messageSpan.End;

            return true;
        }
    }
}
