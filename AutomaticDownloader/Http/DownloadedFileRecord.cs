namespace AutomaticDownloader.Http;

public record DownloadedFileRecord
{
    public bool Status { get; set; }
    public long FileSizeInKb { get; set; }
    public DateTime DateTime { get; set; }
}