using Microsoft.Extensions.Logging;

namespace AutomaticDownloader.Http;


#pragma warning disable S2551 // Shared resources should not be used for locking. Fix in the future
public sealed class MediaDownloaderHelper
{
    private static MediaDownloaderHelper? _singleton;

    private Dictionary<bool, List<MediaDownloaderHelper>> _availabiltyInstanceRecord = new()
    {
        [true] = [],
        [false] = []
    };

    private readonly HttpClient _httpClient;

    private MediaDownloaderHelper()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public static MediaDownloaderHelper Init(int numberOfConcurrentDownloads)
    {
        if (_singleton != null) return _singleton;

        _singleton = new MediaDownloaderHelper();

        for (var i = 0; i < numberOfConcurrentDownloads; i++)
            _singleton._availabiltyInstanceRecord[true].Add(new MediaDownloaderHelper());

        return _singleton;
    }

    public MediaDownloaderHelper? GetConnection()
    {
        lock (this)
        {
            if (_availabiltyInstanceRecord[true].Count == 0) return null;
            
            var downloader = _availabiltyInstanceRecord[true][0];
            _availabiltyInstanceRecord[true].Remove(downloader);
            _availabiltyInstanceRecord[false].Add(downloader);
            return downloader;
        }
    }

    public void FreeConnection(MediaDownloaderHelper mediaDownloader)
    {
        _availabiltyInstanceRecord[false].Remove(mediaDownloader);
        _availabiltyInstanceRecord[true].Add(mediaDownloader);
    }

    public async Task<DownloadedFileRecord> DownloadFile(string siteUrl, string relativeFilePath, string fileName,
        string baseDirectory)
    {
        var fileRecord = new DownloadedFileRecord();
        Program.Logger.LogInformation($"Download started for file {fileName}");
        var downloadFileName = Path.Combine(baseDirectory, fileName);
        var urlParts = siteUrl.Split('/');
        var filePath = string.Join("/", urlParts.Take(urlParts.Length - 2).Append(relativeFilePath[1..]));
        var success = false;
        long sizeOfFile = 0;
        
        try
        {
            Program.Logger.LogInformation($"Download file: {filePath}");
            using var dataStream = await _httpClient.GetStreamAsync(filePath);
            using var fileStream = new FileStream(downloadFileName, FileMode.OpenOrCreate);
            await dataStream.CopyToAsync(fileStream);
            success = true;
            sizeOfFile = new FileInfo(downloadFileName).Length / 1024;
        }
        catch (Exception ex)
        {
            if (File.Exists(downloadFileName))
            {
                sizeOfFile = new FileInfo(downloadFileName).Length / 1024;
                Program.Logger.LogError($"Deleting failed download file {downloadFileName}");
                File.Delete(downloadFileName);
            }
            Program.Logger.LogError(ex.Message, ex.StackTrace);
        }

        fileRecord.Status = success;
        fileRecord.FileSizeInKb = sizeOfFile;
        fileRecord.DateTime = DateTime.Now;
        
        return fileRecord;
    }
}
#pragma warning restore S2551 // Shared resources should not be used for locking