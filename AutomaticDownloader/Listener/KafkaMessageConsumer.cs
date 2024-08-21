using System.Text.Json;
using System;
using AutomaticDownloader.Interfaces;
using Confluent.Kafka;

namespace AutomaticDownloader.Listener
{
    internal sealed class KafkaMessageConsumer : IMessageConsumer
    {
        private readonly KafkaConsumerSettings _settings;
        private Timer? _timer;
        private IConsumer<Ignore, string> _consumer;
        private readonly ConsumerConfig _consumerConfig;

        public KafkaMessageConsumer(KafkaConsumerSettings settings)
        {
            _settings = settings;
            _consumerConfig = _settings.GetConsumerConfig();
            _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        }

        public void StartListening()
        {
            _consumer.Subscribe(_settings.Topic);

            _timer = new Timer(ConsumeKafkaMessages, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private void ConsumeKafkaMessages(object? state)
        {
            try
            {
                var consumeResult = _consumer.Consume(CancellationToken.None);
                if (consumeResult != null)
                {
                    // Decouple later, for now it always be a jsonSerializer
                    var deserializedRecord = JsonSerializer.Deserialize<MediaRecord>(consumeResult.Message.Value);
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");
            }
        }

        public void StopListening()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}
