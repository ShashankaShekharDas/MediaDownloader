using AutomaticDownloader.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
namespace AutomaticDownloader.Listeners
{
    public sealed class KafkaMessageListener : IListener
    {
        private readonly IConfiguration _configuration;
        private const string _topic = "prd.mediadownloader.base";

        public KafkaMessageListener()
        {
            _configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddIniFile("client.properties", false)
                            .Build();

            _configuration["group.id"] = "base-group-test2";
            _configuration["auto.offset.reset"] = "earliest";
        }

#pragma warning disable S2190 // Loops and recursions should not be infinite. Exception as should listen forever
        public void ListenMessage()
#pragma warning restore S2190 // Loops and recursions should not be infinite
        {
            using IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_configuration.AsEnumerable()).Build();
            consumer.Subscribe(_topic);
            while (true)
            {
                try
                {
                    ConsumeResult<string, string> consumedMessage = consumer.Consume();
                    if (consumedMessage != null)
                    {
                        string downloadLink = consumedMessage.Message.Value;
                        Task.Run(() => DownloaderBase.StartDownload(consumedMessage.Message.Key, downloadLink));
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
