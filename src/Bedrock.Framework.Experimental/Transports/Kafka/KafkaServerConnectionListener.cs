using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace Bedrock.Framework
{
    public class KafkaServerConnectionListener : IConnectionListener
    {
        public EndPoint EndPoint => throw new System.NotImplementedException();

        public KafkaServerConnectionListener()
        {
            
        }

        public ValueTask<ConnectionContext> AcceptAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }

        public ValueTask UnbindAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}