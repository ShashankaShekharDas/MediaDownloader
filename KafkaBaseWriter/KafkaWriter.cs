using System.Text.Json;
using Confluent.Kafka;
using KafkaBaseWriter.Helpers;
using Microsoft.Extensions.Configuration;

namespace KafkaBaseWriter;

internal class KafkaWriter
{
    private static void Main(string[] args)
    {
        var config = KafkaConsumerSettings.GetConsumerConfig();
        const string topic = "prd.mediadownloader.relay";

        MediaRecord record = new("Impractical Jokers S09", "https://vadapav.mov/7ef84160-09c6-4aa6-b5c3-2a91dadddd5d/",
            MediaPriority.Medium, DownloadType.Vadapav);
        var serializedString = JsonSerializer.Serialize(record);

        // Send the message
        using var producer = new ProducerBuilder<string, string>(config.AsEnumerable()).Build();

        producer.Flush();

        producer.Produce(topic,
            new Message<string, string>
            {
                Key = record.Name, Value = serializedString,
                Timestamp = new Timestamp(DateTime.Now, TimestampType.CreateTime)
            },
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