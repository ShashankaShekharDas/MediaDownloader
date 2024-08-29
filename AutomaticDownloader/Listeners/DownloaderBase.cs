using AutomaticDownloader.Http;
using AutomaticDownloader.VadapavFileGetter;

namespace AutomaticDownloader.Listeners
{
#pragma warning disable CS8602
    internal sealed class DownloaderBase
    {

        private DownloaderBase()
        {

        }

        async static Task DownLoadFileAsync(string[] linksInfo, MediaFileInfo mediaInfo, MediaDownloaderHelper downloader, string seriesLink)
        {
            MediaDownloaderHelper? connection = null;

            do
            {
                connection = downloader.GetConnection();
            } while (connection is null);

            mediaInfo.MediaLink[linksInfo] = 2;
            Console.WriteLine($"Got connection for {linksInfo[0]} starting now");
            bool result = await connection.DownloadFile(seriesLink, linksInfo[1], linksInfo[0]);
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

            foreach (string[] link in mediaLink.Keys)
            {
                if (mediaLink[link] == 0 || mediaLink[link] == 4)
                {
                    notDownloadedInfo = link;
                    return false;
                }
            }

            return true;
        }

        public static void StartDownload(string name, string link, string baseDirectory)
        {
            Directory.CreateDirectory($"{baseDirectory}\\{name}");
            Console.WriteLine($"Starting downlaod for series {name}");

            MediaDownloaderHelper downloader = MediaDownloaderHelper.Init(3, $"{baseDirectory}\\{name}");
            MediaFileInfo mediaInfo = new MediaDataFetcher(link).GetMediaInfo();

            while (!CheckIfAllFilesDownloaded(mediaInfo.MediaLink, out string[]? downloadLink))
            {
                mediaInfo.MediaLink[downloadLink] = 1;
                Console.WriteLine("Waiting to Start " + downloadLink[0]);
                DownLoadFileAsync(downloadLink, mediaInfo, downloader, link).ConfigureAwait(false);
            }
        }
    }
#pragma warning restore CS8602
}
