using System.Buffers;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    public interface IKafkaMessageReader
    {
        object ReadMessage(ReadOnlySequence<byte> input);
    }
}
