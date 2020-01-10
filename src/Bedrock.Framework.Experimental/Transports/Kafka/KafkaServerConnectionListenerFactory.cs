using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bedrock.Framework.Kafka.Internal;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Bedrock.Framework
{
    public partial class KafkaServerConnectionListenerFactory : IConnectionListenerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly Action<KafkaOptions> _configure;

        public KafkaServerConnectionListenerFactory(ILoggerFactory loggerFactory, Action<KafkaOptions> configure = null)
        {
            _loggerFactory = loggerFactory;
            _configure = configure ?? new Action<KafkaOptions>(_ => { });
        }

        public async ValueTask<IConnectionListener> BindAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var options = new KafkaOptions();

            _configure(options);

            var listener = new KafkaServerConnectionListener(KafkaClient.CreateConsumer(endpoint.ToString(), options));

            await listener.BindAsync(options.Topics, cancellationToken);

            return listener;
        }
    }
}