using Microsoft.Extensions.Configuration;

namespace MediaDownloaderKafkaRelayer.Listener
{
    public sealed class KafkaConsumerSettings
    {
        public static IConfiguration GetConsumerConfig()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddIniFile("client.properties", false)
            .Build();
        }
    }
}
