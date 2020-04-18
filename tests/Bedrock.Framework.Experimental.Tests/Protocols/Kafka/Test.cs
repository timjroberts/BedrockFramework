using System.Runtime.InteropServices;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using System.Threading.Tasks;
using Bedrock.Framework.Experimental.Utils;
using Bedrock.Framework.Protocols;
using Xunit;

namespace Bedrock.Framework.Experimental.Kafka
{
    public class ResponseReader : IMessageReader<Response>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out Response message)
        {
            var reader = new SequenceReader<byte>(input);

            if (!reader.TryReadBigEndian(out int size) || input.Length < size + sizeof(int))
            {
                message = default;

                return false;
            }

            // Could this still fail even though we've ensured that the sequence should contains
            // the bytes we need
            reader.TryReadBigEndian(out short correlationId);

            // Adjust size to remove the bytes occupied by the correlationId
            size -= sizeof(short);

            var messageBytes = input.Slice(reader.Position, size);

            message = new Response(size, correlationId, new Memory<byte>(messageBytes.ToArray()));
            consumed = examined = messageBytes.End;

            return true;
        }
    }

    public readonly struct Response
    {
        public Response(int size, short correlationId, Memory<byte> responseMessage)
        {
            Size = size;
            CorrelationId = correlationId;
            ResponseMessage = responseMessage;
        }

        public readonly int Size;

        public readonly short CorrelationId;

        public readonly Memory<byte> ResponseMessage;
    }

    public class Test
    {
        [Fact]
        public void TestIt()
        {

            var seq = new Sequence<byte>();
            var mem = seq.GetMemory(10);

            BinaryPrimitives.WriteInt32BigEndian(mem.Span, 6);
            BinaryPrimitives.WriteInt16BigEndian(mem.Span.Slice(4), 1);
            BinaryPrimitives.WriteInt32BigEndian(mem.Span.Slice(6), 100);

            seq.Advance(10);

            var reader = new ResponseReader();
            var consumed = new SequencePosition();
            var examined = new SequencePosition();

            reader.TryParseMessage(seq.AsReadOnlySequence, ref consumed, ref examined, out Response resp);
        }

        public async Task<Response> DoIt(Response r)
        {
            return default;
        }
    }
}
