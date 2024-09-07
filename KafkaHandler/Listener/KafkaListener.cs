using Confluent.Kafka;
using KafkaHandler.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KafkaHandler.Listener;

public class KafkaListener : IKafkaListener
{
    public Message<string, string> Listen(IConfiguration configuration, string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(configuration.AsEnumerable()).Build();
        consumer.Subscribe(topic);
        try
        {
            var consumedMessage = consumer.Consume();
            if (consumedMessage is null)
            {
                return Listen(configuration, topic);
            }
            else
            {
                var message = consumedMessage.Message;
                consumer.Commit(consumedMessage);
                return message;
            }
        }
        catch
        {
            Console.WriteLine("Exception");
            return Listen(configuration, topic);
        }
    }
}