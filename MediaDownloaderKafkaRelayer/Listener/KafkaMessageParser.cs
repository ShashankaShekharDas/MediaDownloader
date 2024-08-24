using System.Text.Json;
using Confluent.Kafka;
using MediaDownloaderKafkaRelayer.Helpers;
using MediaDownloaderKafkaRelayer.Interfaces;
using MediaDownloaderKafkaRelayer.Relayer;
using Microsoft.Extensions.Configuration;

namespace MediaDownloaderKafkaRelayer.Listener
{
    internal sealed class KafkaMessageParser : IMessageConsumer
    {
        private readonly IConfiguration _config;
        private readonly string _topic;

        public KafkaMessageParser(string groupId, string offsetReset, string topic)
        {
            _config = KafkaConsumerSettings.GetConsumerConfig();
            _config["group.id"] = groupId;
            _config["auto.offset.reset"] = offsetReset;
            _topic = topic;
        }

        public void StartListening()
        {
            while (true)
            {
                using IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_config.AsEnumerable()).Build();
                consumer.Subscribe(_topic);

                while (true)
                {
                    ConsumeResult<string, string> consumedMessage = consumer.Consume();
                    try
                    {
                        if (consumedMessage != null)
                        {
                            MediaRecord? deserializedRecord = JsonSerializer.Deserialize<MediaRecord>(consumedMessage.Message.Value) ?? throw new InvalidDataException("Invalid value in Kafka topic");

                            string destinationKafkaTopic = string.IsNullOrEmpty(deserializedRecord.ForcedCluster) ? ClusterHelper.GetRelayKafkaTopic(deserializedRecord.Priority)
                                : ClusterHelper.GetRelayKafkaTopic(deserializedRecord.ForcedCluster);

                            // Better to have a common enum for value, download link + torrent info
                            KafkaMessageRelayer.RelayMessage(_config, destinationKafkaTopic, deserializedRecord.Name, deserializedRecord.DownloadInfo);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Exception while reading/parsing");
                    }
                }
            }
        }

        public void StopListening()
        {
        }
    }
}
