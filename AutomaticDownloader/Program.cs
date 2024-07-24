using AutomaticDownloader.FileGetter;
using AutomaticDownloader.Http;

bool AllFilesDownloaded(Dictionary<string[], int>? mediaLink, out string[] notDownloadedInfo)
{
    notDownloadedInfo = [];
    if (mediaLink == null)
    {
        return true;
    }

    foreach (var link in mediaLink.Keys)
    {
        if (mediaLink[link] != 1 && mediaLink[link] != 2)
        {
            notDownloadedInfo = link;
            return false;
        }
    }

    return true;
}

var downloader = MediaDownloader.Init(3, "F:\\Testing");
var mediaInfo = new MediaDataFetcher("https://vadapav.mov/5c9fabd0-1f52-421c-8cd6-3f4d405a3345/").GetMediaInfo();

async Task DownLoadFileAsync(string[] linksInfo)
{
    MediaDownloader? connection = null;
    while (connection is null)
    {
        connection = downloader.GetConnection();
        if (connection is not null)
        {
            break;
        }
    }

    mediaInfo.MediaLink[linksInfo] = 2;
    Console.WriteLine($"Got connection for {linksInfo[0]} starting now");
    var result = await connection.DownloadFile("https://vadapav.mov/0a00527e-99f4-40e8-89da-a30b40f52ac8/", linksInfo[1], linksInfo[0]);
    downloader.FreeConnection(connection);
    mediaInfo.MediaLink[linksInfo] = result ? 3 : 4;
    Console.WriteLine($"{linksInfo[0]} Fiinished with status {mediaInfo.MediaLink[linksInfo]}");
}


while (!AllFilesDownloaded(mediaInfo.MediaLink, out var link))
{
#pragma warning disable CS8602 // WILL NEVER BE NULL AS IF NULL, THE FUNCTION RETURNS FALL
    mediaInfo.MediaLink[link] = 1;
#pragma warning restore CS8602
    Console.WriteLine("Waiting to Start " + link[0]);
    var downloaderThread = new Thread(async () => await DownLoadFileAsync(link));
    downloaderThread.Start();
}