using AutomaticDownloader.Listeners;

namespace AutomaticDownloader
{
    internal static class Program
    {
        static void Main()
        {
            new KafkaMessageListener().ListenMessage();
        }
    }
}