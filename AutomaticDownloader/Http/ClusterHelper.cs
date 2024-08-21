using AutomaticDownloader.Listener;

namespace AutomaticDownloader.Http
{
    // Make it observable, so that clusters will register to it, for now it assumes 2 clusters
    // My local for high priority, and for everything else server
    internal static class ClusterHelper
    {
        public static string GetCluster(MediaPriority priority)
        {
            return priority switch
            {
                MediaPriority.High => "base",
                _ => "lab"
            };
        }
    }
}
