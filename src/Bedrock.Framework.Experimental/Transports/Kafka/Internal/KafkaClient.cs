using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public static string Version => Marshal.GetStringFromUTF8Buffer(KafkaInteropMethods.rd_kafka_version_str());

        private readonly KafkaSafeHandle _kafkaHandle;
        private readonly RdKafkaType _kafkaType;

        private KafkaClient(KafkaSafeHandle kafkaHandle, RdKafkaType kafkaType)
        {
            _kafkaHandle = kafkaHandle;
            _kafkaType = kafkaType;
        }

        /// <summary>
        /// Subscribes the Kafka client to one or more topics.
        /// </summary>
        /// <param name="topics">The topic names that should be subscribed to.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the subscription
        /// operation.</param>
        public async Task Subscribe(IEnumerable<string> topics, CancellationToken cancellationToken = default)
        {
            if (_kafkaType != RdKafkaType.RD_KAFKA_CONSUMER)
                throw new InvalidOperationException("The Kafka client is not configured to operate as a consumer..");

            var topicListHandle = KafkaInteropMethods.rd_kafka_topic_partition_list_new((IntPtr)topics.Count());

            if (topicListHandle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create Kafka topic partition list object.");

            try
            {
                foreach (var topic in topics)
                {
                    KafkaInteropMethods.rd_kafka_topic_partition_list_add(topicListHandle, topic, -1);
                }

                var errorCode = KafkaInteropMethods.rd_kafka_subscribe(_kafkaHandle.DangerousGetHandle(), topicListHandle);

                if (errorCode != RdKafkaErrorCode.NOERROR)
                    throw new InvalidOperationException("Unable to subscribe to specified topics.");
            }
            finally
            {
                KafkaInteropMethods.rd_kafka_topic_partition_list_destroy(topicListHandle);
            }
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
        /// <param name="bootstrapServers">A string representing a comma seperated list of endpoints defining the list of Kafka
        /// brokers to attempt connection to.</param>
        /// <param name="options">Additional Kafka configuration to apply when creating the Kafka client..</param>
        /// <returns>A <see cref="KafkaClient"/> that operates as a Kafka consumer.</returns>
        public static KafkaClient CreateConsumer(string bootstrapServers, KafkaOptions options)
        {
            return CreateKafkaClient(RdKafkaType.RD_KAFKA_CONSUMER, CreateKafkaClientConfig(bootstrapServers, options));
        }

        private static KafkaClient CreateKafkaClient(RdKafkaType type, KafkaConfigSafeHandle configHandle)
        {
            var errStrBuilder = new StringBuilder(KafkaInteropMethods.MaxErrorStringLength);
            var handle = KafkaInteropMethods.rd_kafka_new(type, configHandle.DangerousGetHandle(), errStrBuilder, (UIntPtr)errStrBuilder.Capacity);

            if (handle.IsInvalid)
            {
                // configuration handle will be freed by 'rd_kafka_new' under normal circumstances, but not when
                // there was an error, so free it
                KafkaInteropMethods.rd_kafka_conf_destroy(configHandle.DangerousGetHandle());

                throw new InvalidOperationException(errStrBuilder.ToString());
            }

            return new KafkaClient(handle, type);
        }

        private static KafkaConfigSafeHandle CreateKafkaClientConfig(string bootstrapServers, KafkaOptions options)
        {
            var configHandle = KafkaInteropMethods.rd_kafka_conf_new();

            if (configHandle.IsInvalid)
                throw new InvalidOperationException("Failed to create Kafka configuration object.");

            SetKafkaClientConfigProperty(configHandle, "bootstrap.servers", bootstrapServers);
            SetKafkaClientConfigProperty(configHandle, "group.id", options.GroupId ?? "BedrockConsumer");

            return configHandle;
        }

        private static void SetKafkaClientConfigProperty(KafkaConfigSafeHandle configHandle, string propertyName, string value)
        {
            var errStrBuilder = new StringBuilder(KafkaInteropMethods.MaxErrorStringLength);
            var result = KafkaInteropMethods.rd_kafka_conf_set(configHandle.DangerousGetHandle(), propertyName, value, errStrBuilder, (UIntPtr)errStrBuilder.Capacity);

            switch (result)
            {
                case RdKafkaConfRes.RD_KAFKA_CONF_INVALID:
                    throw new ArgumentException(errStrBuilder.ToString());

                case RdKafkaConfRes.RD_KAFKA_CONF_UNKNOWN:
                    throw new InvalidOperationException(errStrBuilder.ToString());
            }
        }
    }
}