using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    [KakfaMessage(ApiKey.Metadata, Version = 9)]
    public class MetadataMessage : KafkaMessageDescriptor<MetadataRequest, MetadataResponse>
    {
        protected override void WriteRequest(MetadataRequest obj, IBufferWriter<byte> output)
        { }

        protected override MetadataRequest ReadRequest(ReadOnlySequence<byte> input)
        {
            return new MetadataRequest();
        }

        protected override void WriteResponse(MetadataResponse obj, IBufferWriter<byte> output)
        {

        }

        protected override MetadataResponse ReadResponse(ReadOnlySequence<byte> input)
        {
            return new MetadataResponse();
        }
    }
}
