namespace Bedrock.Framework
{
    public static partial class ServerBuilderExtensions
    {
        public static ServerBuilder UseKafka(this ServerBuilder serverBuilder)
        {
            return serverBuilder;
        }

        public static ClientBuilder UseKafka(this ClientBuilder clientBuilder)
        {
            return clientBuilder;
        }
    }
}