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
        if (mediaLink[link] == 0)
        {
            notDownloadedInfo = link;
            return false;
        }
    }

    return true;
}

var downloader = MediaDownloader.Init(3, "F:\\Testing");
var mediaInfo = new MediaDataFetcher("https://vadapav.mov/5c9fabd0-1f52-421c-8cd6-3f4d405a3345/").GetMediaInfo();

while (!AllFilesDownloaded(mediaInfo.MediaLink, out var link))
{

#pragma warning disable CS8602 // WILL NEVER BE NULL AS IF NULL, THE FUNCTION RETURNS FALL
    mediaInfo.MediaLink[link] = 1;
#pragma warning restore CS8602

    MediaDownloader? connection = null;
    while (connection is null)
    {
        connection = downloader.GetConnection();
        if (connection is not null)
        {
            break;
        }
    }

    mediaInfo.MediaLink[link] = 2;
    var result = await connection.DownloadFile("https://vadapav.mov/0a00527e-99f4-40e8-89da-a30b40f52ac8/", link[1], Guid.NewGuid().ToString() + ".mkv");
    mediaInfo.MediaLink[link] = result ? 3 : 4;
}
