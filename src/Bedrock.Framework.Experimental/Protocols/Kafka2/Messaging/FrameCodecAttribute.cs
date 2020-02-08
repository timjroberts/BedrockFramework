using System;
using Bedrock.Framework.Protocols.Kafka2.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka2.Messaging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FrameCodecAttribute : Attribute
    {
        public FrameCodecAttribute(ApiKey apiKey)
        {
            ApiKey = apiKey;
            Version = 0;
        }

        public ApiKey ApiKey { get; }

        public short Version { get; set; }
    }
}
