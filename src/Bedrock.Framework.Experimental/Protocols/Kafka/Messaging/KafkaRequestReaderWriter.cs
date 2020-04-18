using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;
using Bedrock.Framework.Protocols.Kafka.Utils;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public class KafkaRequestReaderWriter : IMessageWriter<KafkaRequest>, IMessageReader<KafkaRequest>
    {
        public void WriteMessage(KafkaRequest message, IBufferWriter<byte> output)
        {
            var size = (int)message.RequestMessage.Length;

            output.WriteInt(size);
            output.WriteShort((short)message.ApiKey);
            output.WriteShort(message.ApiVersion);
            output.WriteInt(message.CorrelationId);
            output.WriteNullableString(message.ClientId);

            message.RequestMessage.CopyTo(output.GetSpan(size));
        }

        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out KafkaRequest message)
        {
            // This will require implementation when the server protocol is implemented
            throw new NotImplementedException();
        }
    }
}
