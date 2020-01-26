using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    [KakfaMessage(ApiKey.ApiVersions)]
    public class ApiVersionsMessage : KafkaMessageDescriptor<Empty, ApiVersionsResponse>
    {
        protected override void WriteRequest(Empty obj, IBufferWriter<byte> output)
        { }

        protected override Empty ReadRequest(ReadOnlySequence<byte> input)
        {
            return Empty.Default;
        }

        protected override void WriteResponse(ApiVersionsResponse obj, IBufferWriter<byte> output)
        {

        }

        protected override ApiVersionsResponse ReadResponse(ReadOnlySequence<byte> input)
        {
            return new ApiVersionsResponse();
        }
    }
}
