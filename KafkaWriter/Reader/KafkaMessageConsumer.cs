using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace KafkaHelper.Reader
{
    public static class KafkaMessageConsumer
    {
        public static string GetMessageFromTopic(IConfiguration config, string topic, string groupId)
        {
            config["group.id"] = groupId;
            config["auto.offset.reset"] = "earliest";

            using (var consumer = new ConsumerBuilder<string, string>(config.AsEnumerable()).Build())
            {
                consumer.Subscribe(topic);
                while (true)
                {
                    // consumes messages from the subscribed topic and prints them to the console
                    var cr = consumer.Consume();
                    return cr.Message.Value;
                }

                // closes the consumer connection
                consumer.Close();
            }
        }
    }
}
