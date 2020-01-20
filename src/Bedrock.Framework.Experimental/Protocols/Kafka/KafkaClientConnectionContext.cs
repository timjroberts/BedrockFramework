using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;
using Microsoft.AspNetCore.Connections;

namespace Bedrock.Framework.Protocols.Kafka
{
    public interface IKafkaConnectionContext
    {
        Task StartAsync(CancellationToken cancellationToken);

        void SendAsync(KafkaRequest request);

        Task<KafkaResponse> SendAsync(KafkaRequest request, CancellationToken cancellationToken);
    }

    public class KafkaClientConnectionContext : IKafkaConnectionContext
    {
        private readonly Channel<KafkaRequest> _requestQueue = Channel.CreateUnbounded<KafkaRequest>(
            new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = false
            });

        private readonly ConnectionContext _connectionContext;

        public KafkaClientConnectionContext(ConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {

        }

        public void SendAsync(KafkaRequest request)
        {
            if (!_requestQueue.Writer.TryWrite(request))
                throw new Exception("Could not send request.");
        }

        public async Task<KafkaResponse> SendAsync(KafkaRequest request, CancellationToken cancellationToken = default)
        {
            _requestQueue.Writer.TryWrite(request);

            return new KafkaResponse();
        }
    }
}
