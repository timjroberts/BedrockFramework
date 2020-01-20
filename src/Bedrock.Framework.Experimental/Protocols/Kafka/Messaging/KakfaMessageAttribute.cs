using System;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KakfaMessageAttribute : Attribute
    {
        public KakfaMessageAttribute(ApiKey apiKey)
        {
            ApiKey = apiKey;
            Version = 0;
        }

        public ApiKey ApiKey { get; }

        public short Version { get; set; }
    }
}
