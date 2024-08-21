using Confluent.Kafka;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace AutomaticDownloader.Listener
{
    internal sealed class KafkaConsumerSettings
    {
        public string BootstrapServers { get; }
        public string Topic { get; }
        public string GroupId { get; }
        public AutoOffsetReset OffsetReset { get; }
        public string DownloadDirectory { get; }

        public KafkaConsumerSettings()
        {
            var kafkaSettings = (NameValueCollection)ConfigurationManager.GetSection("kafkaSettings");
            BootstrapServers = kafkaSettings["BootstrapServers"] ?? string.Empty;
            Topic = kafkaSettings["Topic"] ?? string.Empty;
            GroupId = kafkaSettings["GroupId"] ?? string.Empty;
            OffsetReset = Enum.TryParse(kafkaSettings["OffsetReset"], out AutoOffsetReset offsetReset)
                ? offsetReset
                : AutoOffsetReset.Earliest;
            DownloadDirectory = kafkaSettings["DownloadDirectory"] ?? string.Empty;

            if (string.IsNullOrEmpty(BootstrapServers) || string.IsNullOrEmpty(Topic) || string.IsNullOrEmpty(GroupId))
            {
                throw new InvalidDataException("One of the required fields was empty");
            }
        }

        public ConsumerConfig GetConsumerConfig()
        {
            return new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                GroupId = GroupId,
                AutoOffsetReset = OffsetReset
            };
        }
    }
}
