using Confluent.Kafka;

namespace KafkaHelper.Writer
{
    public sealed class KafkaMessageWriter
    {
        public static async Task<bool> ProduceMessageAsync(ConsumerConfig config, string topic, string message)
        {
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });

                return deliveryResult.Status == PersistenceStatus.Persisted;
            }
            catch (ProduceException<Null, string> exception)
            {
                Console.WriteLine(exception.Message);
            }
            return false;
        }
    }
}
