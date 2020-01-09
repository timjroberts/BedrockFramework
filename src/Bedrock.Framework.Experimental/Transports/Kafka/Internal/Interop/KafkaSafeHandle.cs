using Microsoft.Win32.SafeHandles;

namespace Bedrock.Framework.Kafka.Internal.Interop
{
    public sealed class KafkaSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public KafkaSafeHandle(bool ownsHandle)
            : base(ownsHandle)
        { }

        protected override bool ReleaseHandle()
        {
            KafkaMethods.rd_kafka_destroy(handle);

            return true;
        }
    }
}