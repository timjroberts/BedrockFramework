using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Bedrock.Framework.Kafka.Internal.Interop
{
    public static class KafkaMethods
    {
        public const int MaxErrorStringLength = 512;

        private const string KafkaDllName = "librdkafka";

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr rd_kafka_version_str();

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KafkaSafeHandle rd_kafka_new(RdKafkaType type, IntPtr conf, StringBuilder errstr, UIntPtr errstr_size);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rd_kafka_destroy(IntPtr rk);
    }

    public enum RdKafkaType
    {
        RD_KAFKA_PRODUCER = 0,
        RD_KAFKA_CONSUMER = 1
    }
}