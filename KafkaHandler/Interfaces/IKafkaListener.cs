using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace KafkaHandler.Interfaces;

public interface IKafkaListener
{
    public Message<string, string> Listen(IConfiguration configuration, string topic);
}