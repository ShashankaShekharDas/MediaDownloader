using AutomaticDownloader.Http;

namespace AutomaticDownloader.Interfaces;

public interface ILogger
{
    Task Index(string name, string link, DownloadedFileRecord downloadedFileRecord);
}