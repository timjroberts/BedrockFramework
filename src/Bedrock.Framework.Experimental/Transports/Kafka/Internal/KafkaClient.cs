using System;
using System.Text;
using Bedrock.Framework.Kafka.Internal.Interop;

namespace Bedrock.Framework.Kafka.Internal
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The <see cref="KafkaClient"/> class adapts the imported methods from the underlying Kafka C/C++ client
    /// library ("librdkafka"). It can operate in either a consumer or a producer mode.
    /// </remarks>
    public class KafkaClient : IDisposable
    {
        /// <summary>
        /// Gets the version of the underlying Kafka C/C++ client library ("librdkafka").
        /// </summary>
        /// <returns>A string representing a version.</returns>
        public static string Version => Marshal.GetStringFromUTF8Buffer(KafkaMethods.rd_kafka_version_str());

        private readonly KafkaSafeHandle _kafkaHandle;

        private KafkaClient(KafkaSafeHandle kafkaHandle)
        {
            _kafkaHandle = kafkaHandle;
        }

        /// <summary>
        /// Disposes of the Kafka client.
        /// </summary>
        public void Dispose()
        {
            _kafkaHandle.Dispose();
        }

        /// <summary>
        /// Creates a Kafka client that operates as a consumer.
        /// </summary>
        /// <returns>A <see cref="KafkaClient"/> that operates as a Kafka consumer.</returns>
        public static KafkaClient CreateConsumer()
        {
            return CreateKafkaClient(RdKafkaType.RD_KAFKA_CONSUMER);
        }

        private static KafkaClient CreateKafkaClient(RdKafkaType type)
        {
            var errStrBuilder = new StringBuilder(KafkaMethods.MaxErrorStringLength);
            var handle = KafkaMethods.rd_kafka_new(type, IntPtr.Zero, errStrBuilder, (UIntPtr)errStrBuilder.Capacity);

            if (handle.IsInvalid)
            {
                throw new InvalidOperationException(errStrBuilder.ToString());
            }

            return new KafkaClient(handle);
        }
    }
}