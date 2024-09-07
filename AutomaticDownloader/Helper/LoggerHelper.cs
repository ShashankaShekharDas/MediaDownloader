using AutomaticDownloader.Http;
using Logger.Interfaces;
using Logger.LoggerType;
using Microsoft.Extensions.Logging;

namespace AutomaticDownloader.Helper;

public static class LoggerHelper
{
    public static void FileStatus(bool status, string fileName)
    {
        if (status)
        {
            Program.Logger.LogInformation($"Download Succeeded for File {fileName}");
        }
        else
        {
            Program.Logger.LogError($"Download Failed for File {fileName}");
        }
    }

    public static async Task<bool> InformationLogger(IInformationLogger logger, ElasticLogRecord record) => await logger.AsyncLog(record);

    public static ElasticLogRecord BuildLogRecord(DownloadedFileRecord record, string[] fileNameAndLink)
    {
        return new ElasticLogRecord
        {
            Name = fileNameAndLink[1],
            Link = fileNameAndLink[0],
            Success = record.Status,
            SizeInKb = record.FileSizeInKb
        };
    }
    
}