using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bedrock.Framework.Protocols.Kafka.Utils;
using Bedrock.Framework.Protocols.Kafka2.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka2.Messaging
{
    public interface IFrameCodec
    {
        ApiKey ApiKey { get; }

        short ApiVersion { get; }
    }

    public interface IFrameCodec<TRequest, TResponse> : IFrameCodec
        where TRequest : struct
        where TResponse : struct
    {
        Action<IBufferWriter<byte>> CreateRequestWriter(TRequest request);
        
        TResponse CreateResponseReader(ReadOnlySequence<byte> input);
    }

    public abstract class FrameCodec<TRequest, TResponse> : IFrameCodec<TRequest, TResponse>
        where TRequest : struct
        where TResponse : struct
    {
        protected FrameCodec()
        {
            var attribute = (FrameCodecAttribute)GetType()
                .GetCustomAttributes(typeof(FrameCodecAttribute), true)
                .FirstOrDefault();

            Debug.Assert(attribute is object, $"Expected '{nameof(FrameCodecAttribute)}' to have been applied to type '{GetType().Name}'.");

            ApiKey = attribute.ApiKey;
            ApiVersion = attribute.Version;
        }

        public ApiKey ApiKey { get; private set; }

        public short ApiVersion { get; private set; }

        public Action<IBufferWriter<byte>> CreateRequestWriter(TRequest request)
        {
            int size = OnCalculateRequestInstanceSize(request);
            short apiKey = (short)ApiKey;
            short apiVersion = ApiVersion;
            int correlationId = 100;

            return (writer) =>
            {
                writer.WriteInt(size);
                writer.WriteShort(apiKey);
                writer.WriteShort(apiVersion);
                writer.WriteInt(correlationId);
                writer.WriteNullableString("StreamR");
                OnWriteRequestInstance(request, writer);
            };
        }

        public TResponse CreateResponseReader(ReadOnlySequence<byte> input)
        {
            var reader = new SequenceReader<byte>(input);

            reader.TryReadInt(out int size);
            reader.TryReadShort(out short correlationId);
            return OnReadResponseInstance(reader);
        }

        protected abstract int OnCalculateRequestInstanceSize(in TRequest request);

        protected abstract void OnWriteRequestInstance(in TRequest request, in IBufferWriter<byte> writer);

        protected abstract int OnCalculateResponseInstanceSize(in TResponse response);

        protected abstract void OnWriteResponseInstance(in TResponse response, in IBufferWriter<byte> writer);

        protected abstract TResponse OnReadResponseInstance(SequenceReader<byte> reader);
    }

    public interface IFrameCodecFactory
    {
        bool TryGetFrameCodec<TRequest, TResponse>(out IFrameCodec<TRequest, TResponse> frameCodec)
            where TRequest : struct
            where TResponse : struct;

        bool TryGetFrameCodec<TRequest, TResponse>(short apiKeyVersion, out IFrameCodec<TRequest, TResponse> frameCodec)
            where TRequest : struct
            where TResponse : struct;
    }

    public class FrameCodecFactory : IFrameCodecFactory
    {
        private readonly IEnumerable<IFrameCodec> _frameCodecs;

        public FrameCodecFactory(IEnumerable<IFrameCodec> frameCodecs)
        {
            _frameCodecs = frameCodecs;
        }

        public bool TryGetFrameCodec<TRequest, TResponse>(out IFrameCodec<TRequest, TResponse> frameCodec)
            where TRequest : struct
            where TResponse : struct
        {
            return TryGetFrameCodec(ApiKeyVersion.Latest, out frameCodec);
        }

        public bool TryGetFrameCodec<TRequest, TResponse>(short apiKeyVersion, out IFrameCodec<TRequest, TResponse> frameCodec)
            where TRequest : struct
            where TResponse : struct
        {
            throw new System.NotImplementedException();
        }
    }
}
