using MediaDownloaderKafkaRelayer.Listener;

namespace MediaDownloaderKafkaRelayer;

internal class Program
{
    public static void Main()
    {
        var topic = Environment.GetEnvironmentVariable("topic") ?? "prd.mediadownloader.relay";
        Console.WriteLine($"Will try to listen to topic {topic}");
        new KafkaMessageParser(topic, "earliest", topic).StartListening();
    }
}