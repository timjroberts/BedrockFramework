using System.Buffers;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public interface IKafkaMessageWriter
    {
        void WriteMessage(object obj, IBufferWriter<byte> output);
    }
}
