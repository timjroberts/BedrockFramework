namespace Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes
{
    public struct MetadataRequest
    {
        public Topic[] Topics;

        public bool AllowAutoTopicCreation;

        public bool IncludeClusterAuthorizedOperations;

        public bool IncludeTopicAuthorizedOperations;
    }
}
