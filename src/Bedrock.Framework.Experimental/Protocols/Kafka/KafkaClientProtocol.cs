using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols.Kafka.Messaging;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;
using Microsoft.AspNetCore.Connections;

namespace Bedrock.Framework.Protocols.Kafka
{
    public class KafkaClientProtocol
    {
        private readonly KafkaClientConnectionContext _connectionContext;
        
        public KafkaClientProtocol(ConnectionContext connectionContext)
        {
            _connectionContext = new KafkaClientConnectionContext(connectionContext);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _ = _connectionContext.StartAsync(cancellationToken);
        }
    }
}
