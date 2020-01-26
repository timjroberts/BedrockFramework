using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public class KafkaRequestWriter : IMessageWriter<KafkaRequest>
    {
        public void WriteMessage(KafkaRequest message, IBufferWriter<byte> output)
        {
            using var buffer = new MemoryBufferWriter<byte>();

            buffer.WriteShort((short)message.ApiKey);
            buffer.WriteShort(message.ApiVersion);
            buffer.WriteInt(message.CorrelationId);
            buffer.WriteNullableString(message.ClientId);
            message.RequestWriter.WriteMessage(message.RequestMessage, buffer);

            output.WriteInt((int)buffer.Length);
            buffer.CopyTo(output);
        }
    }
}
