namespace AutomaticDownloader.VadapavFileGetter;

public record MediaFileInfo
{
    public string? Title { get; set; }
    public string? Season { get; set; }

    public Dictionary<string[], int>? MediaLink { get; set; }
    // string[] => [fileName, downloadLink]
    // Int -> Status
    // 0 -> Not started
    // 1 -> Waiting for connection
    // 2 -> Downloading
    // 3 -> Completed
    // 4 -> Errored
}