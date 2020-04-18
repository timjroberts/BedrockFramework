using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace Bedrock.Framework.Protocols.Kafka
{
    public class KafkaClientProtocol
    {
        private readonly IKafkaClientConnectionContext _connectionContext;
        
        public KafkaClientProtocol(IKafkaClientConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
        }

        public Task StartAsync(ConnectionContext connectionContext, CancellationToken cancellationToken = default)
        {
            return _connectionContext.StartAsync(connectionContext, cancellationToken);
        }
    }
}
