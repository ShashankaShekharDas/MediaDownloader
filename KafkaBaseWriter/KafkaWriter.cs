using System.Text.Json;
using Confluent.Kafka;
using KafkaBaseWriter.Helpers;
using Microsoft.Extensions.Configuration;

namespace KafkaBaseWriter
{
    internal class KafkaWriter
    {
        static void Main(string[] args)
        {
            IConfiguration config = KafkaConsumerSettings.GetConsumerConfig();
            const string topic = "prd.mediadownloader.relay";

            MediaRecord record = new("Bad Teacher", "https://vadapav.mov/7d3d0641-ea62-4b83-a5fa-c70477fd641e/", MediaPriority.High, DownloadType.Vadapav);

            string serializedString = JsonSerializer.Serialize(record);

            // Send the message
            using IProducer<string, string> producer = new ProducerBuilder<string, string>(config.AsEnumerable()).Build();

            producer.Produce(topic, new Message<string, string> { Key = record.Name, Value = serializedString, Timestamp = new Timestamp(DateTime.Now, TimestampType.CreateTime) },
                 (deliveryReport) =>
                 {
                     if (deliveryReport.Error.Code != ErrorCode.NoError)
                     {
                         Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                     }
                     else
                     {
                         Console.WriteLine($"Produced event to topic {topic}: key = {deliveryReport.Message.Key,-10} value = {deliveryReport.Message.Value}");
                     }
                 }
               );

            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
