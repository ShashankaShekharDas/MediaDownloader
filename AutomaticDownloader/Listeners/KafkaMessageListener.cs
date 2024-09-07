using AutomaticDownloader.Interfaces;
using Confluent.Kafka;
using KafkaHandler.Helpers;
using KafkaHandler.Listener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutomaticDownloader.Listeners;
public sealed class KafkaMessageListener : IListener
{
    private readonly IConfiguration _configuration;
    private const string Directory = "/download";
    private readonly string _topic;
    private readonly int _concurrentDownloads;
    private readonly KafkaListener _kafkaListener;

    public KafkaMessageListener(string topic, int concurrentDownloads)
    {
        _topic = topic;
        _configuration = KafkaBaseHandler.GetConfiguration(_topic);
        _kafkaListener = new KafkaListener();
        _concurrentDownloads = concurrentDownloads;
    }

#pragma warning disable S2190
    public void ListenMessage()
    {
        Program.Logger.LogInformation($"Started listening on topic {_topic}");
        while (true)
        {
            try
            {
                var consumedMessage = _kafkaListener.Listen(_configuration, _topic);
                var downloadLink = consumedMessage.Value;
                Program.Logger.LogInformation($"Got message. Parsing it and will start download. Key {consumedMessage.Key} Value {downloadLink}");
                Task.Run(() => DownloaderBase.StartDownload(consumedMessage.Key, downloadLink, Directory, _concurrentDownloads));
            }
            catch
            {
                // ignored
            }
        }
    }
#pragma warning restore S2190
}