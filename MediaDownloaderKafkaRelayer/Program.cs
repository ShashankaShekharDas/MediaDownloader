using MediaDownloaderKafkaRelayer.Listener;

namespace MediaDownloaderKafkaRelayer
{
    class Program
    {
        public static void Main(string[] args)
        {
            string topic = Environment.GetEnvironmentVariable("topic") ?? "prd.mediadownloader.relay";

            new KafkaMessageParser(topic, "earliest", topic).StartListening();
        }
    }
}
