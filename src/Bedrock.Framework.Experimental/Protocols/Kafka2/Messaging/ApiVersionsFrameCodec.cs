using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Utils;
using Bedrock.Framework.Protocols.Kafka2.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka2.Messaging
{
    [FrameCodec(ApiKey.ApiVersions, Version = 0)]
    public class ApiVersionsFrameCodec0 : FrameCodec<ApiVersionsRequest, ApiVersionsResponse>
    {
        protected override int OnCalculateRequestInstanceSize(in ApiVersionsRequest request)
        {
            return 0; // Version 0 of ApiVersions request has no output
        }

        protected override int OnCalculateResponseInstanceSize(in ApiVersionsResponse response)
        {
            return
                sizeof(short) +
                response.ApiKeys.Length * (
                    sizeof(ApiKey) +
                    sizeof(short) +
                    sizeof(short));
        }

        protected override void OnWriteRequestInstance(in ApiVersionsRequest request, in IBufferWriter<byte> writer)
        {
            // Version 0 of ApiVersions request has no output
        }

        protected override void OnWriteResponseInstance(in ApiVersionsResponse response, in IBufferWriter<byte> writer)
        {
        }

        protected override ApiVersionsResponse OnReadResponseInstance(SequenceReader<byte> reader)
        {
            reader.TryReadShort(out short errorCode);
            reader.TryReadArray<ApiKeyVersion>(null, out ApiKeyVersion[] apiKeys);

            return new ApiVersionsResponse()
            {
                ErrorCode = errorCode,
                ApiKeys = apiKeys
            };
        }
    }
}
