using MediaDownloaderKafkaRelayer.Listener;

namespace MediaDownloaderKafkaRelayer.Helpers
{
    // Make it observable, so that clusters will register to it, for now it assumes 2 clusters
    // My local for high priority, and for everything else server
    // Receiver : prd.mediadownloader.relay
    // Kafka topic names -> prd.mediadownloader.<machine-name>
    internal static class ClusterHelper
    {
        public static string GetRelayKafkaTopic(MediaPriority priority)
        {
            return priority switch
            {
                MediaPriority.High => "prd.mediadownloader.base",
                _ => "prd.mediadownloader.lab"
            };
        }
        public static string GetRelayKafkaTopic(string forcedCluster)
        {
            return forcedCluster switch
            {
                "base" => "prd.mediadownloader.base",
                _ => "prd.mediadownloader.lab"
            };
        }
    }
}
