using AutomaticDownloader.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
namespace AutomaticDownloader.Listeners
{
    public sealed class KafkaMessageListener : IListener
    {
        private readonly IConfiguration _configuration;
        private readonly string _topic = Environment.GetEnvironmentVariable("topic") ?? "prd.mediadownloader.base";
        private readonly string _directory = Environment.GetEnvironmentVariable("baseDirectory") ?? "D:\\Qbit";

        public KafkaMessageListener()
        {
            _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddIniFile("client.properties", false)
                            .Build();

            _configuration["group.id"] = _topic;
            _configuration["auto.offset.reset"] = "earliest";
        }

#pragma warning disable S2190 // Loops and recursions should not be infinite. Exception as should listen forever
        public void ListenMessage()
#pragma warning restore S2190 // Loops and recursions should not be infinite
        {
            Console.WriteLine($"Started listening on topic {_topic}");
            using IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_configuration.AsEnumerable()).Build();
            consumer.Subscribe(_topic);
            while (true)
            {
                try
                {
                    ConsumeResult<string, string> consumedMessage = consumer.Consume();
                    if (consumedMessage != null)
                    {
                        Console.WriteLine("Got message. Parsing it and will start download");
                        string downloadLink = consumedMessage.Message.Value;
                        Task.Run(() => DownloaderBase.StartDownload(consumedMessage.Message.Key, downloadLink, _directory));
                    }
                }
                catch
                {
                    Console.WriteLine("Exception");
                }
            }
        }
    }
}
