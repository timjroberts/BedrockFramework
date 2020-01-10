using Microsoft.Win32.SafeHandles;

namespace Bedrock.Framework.Kafka.Internal.Interop
{
    public sealed class KafkaConfigSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public KafkaConfigSafeHandle(bool ownsHandle)
            : base(ownsHandle)
        { }

        protected override bool ReleaseHandle()
        {
            KafkaInteropMethods.rd_kafka_conf_destroy(handle);

            return true;
        }
    }
}