using Microsoft.Extensions.Configuration;

namespace KafkaBaseWriter.Helpers;

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