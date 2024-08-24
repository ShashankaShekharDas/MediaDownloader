using System.Text.Json;
using MediaDownloaderKafkaRelayer.Listener;
using MediaDownloaderKafkaRelayer.Relayer;
using Microsoft.Extensions.Configuration;

namespace MediaDownloaderKafkaRelayer
{
    class Program
    {
        public static void Main(string[] args)
        {
            // producer and consumer code here
            IConfiguration config = KafkaConsumerSettings.GetConsumerConfig();
            const string topic = "prd.mediadownloader.relay";

            MediaRecord testRecord = new MediaRecord("Game of Thrones S04", "https://vadapav.mov/bd33533e-e20d-440d-9b24-d680475f1ff1/", MediaPriority.High, DownloadType.Vadapav);

            string serializedString = JsonSerializer.Serialize(testRecord);

            KafkaMessageRelayer.RelayMessage(config, topic, testRecord.Name, serializedString);

            new KafkaMessageParser("TEST_GROUP", "earliest", topic).StartListening();

        }
    }
}
