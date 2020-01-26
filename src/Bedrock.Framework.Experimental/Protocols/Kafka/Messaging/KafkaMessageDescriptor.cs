using System;
using System.Buffers;
using Bedrock.Framework.Protocols.Kafka.Messaging.SchemaTypes;

namespace Bedrock.Framework.Protocols.Kafka.Messaging
{
    /// <summary>
    /// Describes an API message that can be used as part of the wire protocol between a Kafka client and broker.
    /// </summary>
    public interface IKafkaMessageDescriptor
    {
        ApiKey ApiKey { get; }

        short ApiVersion { get; }

        IKafkaMessageWriter GetRequestWriter();

        IKafkaMessageReader GetRequestReader();

        IKafkaMessageWriter GetResponseWriter();

        IKafkaMessageReader GetResponseReader();
    }

    public interface IKafkaMessageWriter
    {
        void WriteMessage(object obj, IBufferWriter<byte> output);
    }

    public interface IKafkaMessageReader
    {
        object ReadMessage(ReadOnlySequence<byte> input);
    }

    /// <summary>
    /// Describes an API message that can be used as part of the wire protocol between a Kafka client and broker.
    /// </summary>
    /// <typeparam name="TRequestSchema">The request schema type.</typeparam>
    /// <typeparam name="TResponseSchema">The response schema type.</typeparam>
    /// <remarks>
    /// A message descriptor brings together a request and response schema type for a version of a Kafka API
    /// message that is implemented by a Kafka broker. The request and response schema types are inclusive of all
    /// prior versions of the message up to and including the highest version supported by this protocol implementation.
    /// 
    /// The highest version of a message supported by both Kafka client and broker should be used.
    /// </remarks>
    public abstract class KafkaMessageDescriptor<TRequestSchema, TResponseSchema> : IKafkaMessageDescriptor
    {
        private class KafkaMessageWriter<TSchema> : IKafkaMessageWriter
        {
            private readonly Action<TSchema, IBufferWriter<byte>> _writer;

            public KafkaMessageWriter(Action<TSchema, IBufferWriter<byte>> writer)
            {
                _writer = writer;
            }

            public void WriteMessage(object obj, IBufferWriter<byte> output)
            {
                _writer.Invoke((TSchema)obj, output);
            }
        }

        private class KafkaMessagReader<TSchema> : IKafkaMessageReader
        {
            private readonly Func<ReadOnlySequence<byte>, TSchema> _reader;

            public KafkaMessagReader(Func<ReadOnlySequence<byte>, TSchema> reader)
            {
                _reader = reader;
            }

            public object ReadMessage(ReadOnlySequence<byte> input)
            {
                return _reader.Invoke(input);
            }
        }

        protected KafkaMessageDescriptor()
        {

        }

        public ApiKey ApiKey { get; private set; }

        public short ApiVersion { get; private set; }

        public IKafkaMessageWriter GetRequestWriter()
        {
            return new KafkaMessageWriter<TRequestSchema>(WriteRequest);
        }

        public IKafkaMessageReader GetRequestReader()
        {
            return new KafkaMessagReader<TRequestSchema>(ReadRequest);
        }

        public IKafkaMessageWriter GetResponseWriter()
        {
            return new KafkaMessageWriter<TResponseSchema>(WriteResponse);
        }

        public IKafkaMessageReader GetResponseReader()
        {
            return new KafkaMessagReader<TResponseSchema>(ReadResponse);
        }

        protected abstract void WriteRequest(TRequestSchema obj, IBufferWriter<byte> output);

        protected abstract TRequestSchema ReadRequest(ReadOnlySequence<byte> input);

        protected abstract void WriteResponse(TResponseSchema obj, IBufferWriter<byte> output);

        protected abstract TResponseSchema ReadResponse(ReadOnlySequence<byte> input);
    }
}
