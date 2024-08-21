

using AutomaticDownloader.FileGetter;
using AutomaticDownloader.Http;
using AutomaticDownloader.VadapavFileGetter;


internal class Program
{
#pragma warning disable CS8602
    async static Task DownLoadFileAsync(string[] linksInfo, MediaFileInfo mediaInfo, MediaDownloaderHelper downloader, string seriesLink)
    {
        MediaDownloaderHelper? connection = null;
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
        var result = await connection.DownloadFile(seriesLink, linksInfo[1], linksInfo[0]);
        downloader.FreeConnection(connection);
        mediaInfo.MediaLink[linksInfo] = result ? 3 : 4;
        Console.WriteLine($"{linksInfo[0]} Finished with status {mediaInfo.MediaLink[linksInfo]}");
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
            if (mediaLink[link] == 0 || mediaLink[link] == 4)
            {
                notDownloadedInfo = link;
                return false;
            }
        }

        return true;
    }

    public static void StartDownload(string seriesLink, MediaDownloaderHelper downloader, MediaFileInfo mediaInfo)
    {
        while (!CheckIfAllFilesDownloaded(mediaInfo.MediaLink, out var link))
        {
            mediaInfo.MediaLink[link] = 1;
            Console.WriteLine("Waiting to Start " + link[0]);
            var downloaderThread = new Thread(async () =>  await DownLoadFileAsync(link, mediaInfo, downloader, seriesLink));
            downloaderThread.Start();
            downloaderThread.Join();
        }
    }

    private async static Task Main()
    {
        var baseDirectory = "D:\\Qbit";
        var seriesLinkDictionary = new Dictionary<string, string>
        {
            ["Game of Thrones"] = "https://vadapav.mov/4d27c257-db29-4a8e-bc66-c670dbd82930/"
        };

        foreach (var seriesName in seriesLinkDictionary.Keys)
        {
            Directory.CreateDirectory($"{baseDirectory}\\{seriesName}");
            Console.WriteLine($"Starting downlaod for series {seriesName}");
            var downloader = MediaDownloaderHelper.Init(3, $"{baseDirectory}\\{seriesName}");
            var seriesLink = seriesLinkDictionary[seriesName];
            var mediaInfo = new MediaDataFetcher(seriesLink).GetMediaInfo();

            while (!CheckIfAllFilesDownloaded(mediaInfo.MediaLink, out var link))
            {
                mediaInfo.MediaLink[link] = 1;
                Console.WriteLine("Waiting to Start " + link[0]);
                await DownLoadFileAsync(link, mediaInfo, downloader, seriesLink).ConfigureAwait(false);
            }
        }
    }
#pragma warning restore CS8602 
}
