using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Kafka.Internal;
using Microsoft.AspNetCore.Connections;

namespace Bedrock.Framework
{
    public class KafkaServerConnectionListener : IConnectionListener
    {
        private readonly KafkaClient _kafkaClient;
        
        public EndPoint EndPoint => throw new System.NotImplementedException();

        public KafkaServerConnectionListener(KafkaClient kafkaClient)
        {
            _kafkaClient = kafkaClient;
        }

        public Task BindAsync(IEnumerable<string> topics, CancellationToken cancellationToken = default)
        {
            return _kafkaClient.Subscribe(topics, cancellationToken);
        }

        public ValueTask<ConnectionContext> AcceptAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask DisposeAsync()
        {
            await UnbindAsync().ConfigureAwait(false);

            _kafkaClient.Dispose();
        }

        public async ValueTask UnbindAsync(CancellationToken cancellationToken = default)
        {
            
        }
    }
}