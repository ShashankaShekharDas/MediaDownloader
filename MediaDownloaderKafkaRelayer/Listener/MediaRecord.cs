namespace MediaDownloaderKafkaRelayer.Listener
{
    public record MediaRecord(string Name, string DownloadInfo, MediaPriority Priority, DownloadType Type)
    {
        public string Name { get; set; } = Name;
        public string DownloaderInfo { get; set; } = DownloadInfo;
        public MediaPriority Priority { get; set; } = Priority;
        public string? ForcedCluster { get; set; }
        public DownloadType DownloadType { get; set; } = Type;
    }
}
