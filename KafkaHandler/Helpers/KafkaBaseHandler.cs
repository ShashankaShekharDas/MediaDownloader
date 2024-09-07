using Microsoft.Extensions.Configuration;

namespace KafkaHandler.Helpers;

public static class KafkaBaseHandler
{
    public static IConfiguration GetConfiguration(string topic)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddIniFile("client.properties", false)
            .Build();
        config["group.id"] = topic;
        config["auto.offset.reset"] = "earliest";

        return config;
    }
}