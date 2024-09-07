using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace MediaDownloaderKafkaRelayer.Relayer;

public sealed class KafkaMessageRelayer
{
    public static void RelayMessage(IConfiguration config, string topic, string key, string message)
    {
        using var producer = new ProducerBuilder<string, string>(config.AsEnumerable()).Build();

        producer.Produce(topic,
            new Message<string, string>
                { Key = key, Value = message, Timestamp = new Timestamp(DateTime.Now, TimestampType.CreateTime) },
            deliveryReport =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                    Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                else
                    Console.WriteLine(
                        $"Produced event to topic {topic}: key = {deliveryReport.Message.Key,-10} value = {deliveryReport.Message.Value}");
            }
        );

        producer.Flush(TimeSpan.FromSeconds(10));
    }
}