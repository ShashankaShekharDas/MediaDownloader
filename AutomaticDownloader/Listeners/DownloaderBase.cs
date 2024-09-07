using AutomaticDownloader.Helper;
using AutomaticDownloader.Http;
using AutomaticDownloader.VadapavFileGetter;
using Logger.Elastic;
using Logger.Interfaces;
using Microsoft.Extensions.Logging;

namespace AutomaticDownloader.Listeners;
#pragma warning disable CS8602
internal static class DownloaderBase
{
    private static readonly IInformationLogger _logger = new ElasticSearchInformationLogger();

    private static async Task DownLoadFileAsync(string[] linksInfo, MediaFileInfo mediaInfo, MediaDownloaderHelper downloader, string seriesLink, string directory)
    {
        MediaDownloaderHelper? connection;

        do
        {
            connection = downloader.GetConnection();
        } while (connection is null);

        mediaInfo.MediaLink[linksInfo] = 2;
        Program.Logger.LogInformation($"Got connection for {linksInfo[0]} starting now");
        
        var result = await connection.DownloadFile(seriesLink, linksInfo[1], linksInfo[0], directory);
        downloader.FreeConnection(connection);
        mediaInfo.MediaLink[linksInfo] = result.Status ? 3 : 4;

       LoggerHelper.FileStatus(result.Status, linksInfo[0]);
       await LoggerHelper.InformationLogger(_logger, LoggerHelper.BuildLogRecord(result, linksInfo));

       Program.Logger.LogInformation($"{linksInfo[0]} Finished with status {mediaInfo.MediaLink[linksInfo]}");
    }

    private static bool CheckIfAllFilesDownloaded(Dictionary<string[], int>? mediaLink, out string[] notDownloadedInfo)
    {
        notDownloadedInfo = [];
        if (mediaLink is null) return true;

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

    public static void StartDownload(string name, string link, string baseDirectory, int concurrentDownloads)
    {
        try
        {
            var downloadDirectory = Path.Join(baseDirectory, name);
            Program.Logger.LogInformation($"Starting download for series {name}");
            Directory.CreateDirectory(downloadDirectory);
            var downloader = MediaDownloaderHelper.Init(concurrentDownloads);
            var mediaInfo = new MediaDataFetcher(link).GetMediaInfo();

            while (!CheckIfAllFilesDownloaded(mediaInfo.MediaLink, out var downloadLink))
            {
                mediaInfo.MediaLink[downloadLink] = 1;
                Program.Logger.LogInformation($"Waiting to Start {downloadLink[0]}");
                DownLoadFileAsync(downloadLink, mediaInfo, downloader, link, downloadDirectory).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Program.Logger.LogError(e.StackTrace);
        }
    }
}
#pragma warning restore CS8602