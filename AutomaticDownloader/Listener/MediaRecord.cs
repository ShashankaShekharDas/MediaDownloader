namespace AutomaticDownloader.Listener
{
    public record MediaRecord(string Name, string Link, MediaPriority Priority)
    {
        public string Name { get; set; } = Name;
        public string DownloadLink { get; set; } = Link;
        public MediaPriority Priority { get; set; } = Priority;
        public string? ForcedCluster { get; set; }
    }
}
