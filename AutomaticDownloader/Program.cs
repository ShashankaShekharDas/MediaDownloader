using AutomaticDownloader.FileGetter;
using AutomaticDownloader.Http;


internal class Program
{
    async static Task DownLoadFileAsync(string[] linksInfo, AutomaticDownloader.VadapavFileGetter.MediaFileInfo mediaInfo, MediaDownloader downloader, string seriesLink)
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

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        mediaInfo.MediaLink[linksInfo] = 2;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        Console.WriteLine($"Got connection for {linksInfo[0]} starting now");
        var result = await connection.DownloadFile(seriesLink, linksInfo[1], linksInfo[0]);
        downloader.FreeConnection(connection);
        mediaInfo.MediaLink[linksInfo] = result ? 3 : 4;
        Console.WriteLine($"{linksInfo[0]} Fiinished with status {mediaInfo.MediaLink[linksInfo]}");
    }

    static bool CheckIfAllFilesDownloaded(Dictionary<string[], int>? mediaLink, out string[] notDownloadedInfo)
    {
        notDownloadedInfo = [];
        if (mediaLink is null)
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

    public static void AsyncMain(string seriesLink, MediaDownloader downloader, AutomaticDownloader.VadapavFileGetter.MediaFileInfo mediaInfo)
    {
        while (!CheckIfAllFilesDownloaded(mediaInfo.MediaLink, out var link))
        {
#pragma warning disable CS8602 // WILL NEVER BE NULL AS IF NULL, THE FUNCTION RETURNS FALL
            mediaInfo.MediaLink[link] = 1;
#pragma warning restore CS8602

            Console.WriteLine("Waiting to Start " + link[0]);
            var downloaderThread = new Thread(async () => await DownLoadFileAsync(link, mediaInfo, downloader, seriesLink));
            downloaderThread.Start();
        }
    }

    private static void Main()
    {
        var baseDirectory = "F:\\Testing";
        var seriesLinkDictionary = new Dictionary<string, string>{
            ["BlackList-S05"] = "https://vadapav.mov/f3ae5c5b-5bfe-46b5-83ed-ae88c847f571/",
            ["BlackList-S06"] = "https://vadapav.mov/601c9118-04bf-46ee-a4a5-2a9454d4a9fc/",
            ["BlackList-S07"] = "https://vadapav.mov/65ba9f44-6da3-4c9f-919c-c7145ff216fa/",
            ["BlackList-S08"] = "https://vadapav.mov/a42c31ed-7548-47ce-a0cb-904a3683e420/",
            ["BlackList-S09"] = "https://vadapav.mov/0f876b9e-a6c9-466d-ae0a-9fe0550a9d31/",
            ["BlackList-S10"] = "https://vadapav.mov/70258bc4-2af4-4020-a422-eac4df519c81/",

        };

        foreach (var seriesName in seriesLinkDictionary.Keys)
        {
            Directory.CreateDirectory($"{baseDirectory}\\{seriesName}");
            // This essentially means 3 downlaod for each season, needs to separate
            var downloader = MediaDownloader.Init(3, $"{baseDirectory}\\{seriesName}");
            Console.WriteLine($"Starting downlaod for series {seriesName}");

            var seriesLink = seriesLinkDictionary[seriesName];
            var mediaInfo = new MediaDataFetcher(seriesLink).GetMediaInfo();

            // Bad workaround for now, to restart failed downloads
            while (!CheckIfAllFilesDownloaded(mediaInfo.MediaLink, out var _))
            {
                AsyncMain(seriesLink, downloader, mediaInfo);
            }
        }
    }
}