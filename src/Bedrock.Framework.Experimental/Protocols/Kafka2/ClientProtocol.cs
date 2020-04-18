using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols.Kafka2.Messaging;
using Bedrock.Framework.Protocols.Kafka2.Messaging.SchemaTypes;
using Microsoft.AspNetCore.Connections;

namespace Bedrock.Framework.Protocols.Kafka2
{
    public class ClientProtocol
    {
        private static readonly DelegatedBufferWriter BufferWriter = new DelegatedBufferWriter();

        private readonly Channel<RequestContext> _requestChannel = Channel.CreateUnbounded<RequestContext>(
            new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = false
            }
        );

        private readonly IFrameCodecFactory _frameCodecFactory;

        public ClientProtocol(IFrameCodecFactory frameCodecFactory)
        {
            _frameCodecFactory = frameCodecFactory;
        }

        public async Task StartAsync(ConnectionContext connection, CancellationToken cancellationToken = default)
        {
            _ = WriteRequestsAsync(connection, cancellationToken);

            var foo = await QuerySupportedApiVersionsAsync().ConfigureAwait(false);
        }

        private async Task<string> QuerySupportedApiVersionsAsync()
        {
            var apiVersionsRequest = new ApiVersionsRequest();
            var apiVersionsResponse = await SendAsync<ApiVersionsRequest, ApiVersionsResponse>(apiVersionsRequest).ConfigureAwait(false);

            return string.Empty;
        }

        private Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
            where TRequest : struct
            where TResponse : struct
        {
            if (!_frameCodecFactory.TryGetFrameCodec(ApiKeyVersion.Latest, out IFrameCodec<TRequest, TResponse> frameCodec))
                throw new NotImplementedException();

            return SendAsync(request, frameCodec);
        }

        private async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, IFrameCodec<TRequest, TResponse> frameCodec)
            where TRequest : struct
            where TResponse : struct
        {
            var tcs = new TaskCompletionSource<IMemoryOwner<byte>>();

            await _requestChannel.Writer.WriteAsync(new RequestContext(tcs, frameCodec.CreateRequestWriter(request)));

            using (var ownedMemory = await tcs.Task)
            {
                return frameCodec.CreateResponseReader(new ReadOnlySequence<byte>(ownedMemory.Memory));
            }
        }

        private async Task WriteRequestsAsync(ConnectionContext connection, CancellationToken cancellationToken = default)
        {
            var writer = connection.CreateWriter();

            while (await _requestChannel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_requestChannel.Reader.TryRead(out RequestContext requestContext))
                {
                    await writer.WriteAsync(BufferWriter, requestContext.RequestBufferWriter, cancellationToken);
                }
            }
        }

        private class DelegatedBufferWriter : IMessageWriter<Action<IBufferWriter<byte>>>
        {
            public void WriteMessage(Action<IBufferWriter<byte>> writer, IBufferWriter<byte> output)
            {
                writer(output);
            }
        }

        private struct RequestContext
        {
            public RequestContext(TaskCompletionSource<IMemoryOwner<byte>> responseCompletionSource, Action<IBufferWriter<byte>> requestBufferWriter)
            {
                ResponseCompletionSource = responseCompletionSource;
                RequestBufferWriter = requestBufferWriter;
            }

            public TaskCompletionSource<IMemoryOwner<byte>> ResponseCompletionSource;

            public Action<IBufferWriter<byte>> RequestBufferWriter;
        }
    }
}
