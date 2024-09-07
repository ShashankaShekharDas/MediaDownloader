using Logger.Elastic;

namespace Logger.LoggerType;

public class ElasticLogRecord : IMediaLoggerType
{
    public string? Name { get; set; }
    public string? Link { get; set; }
    public bool Success { get; set; }
    public long SizeInKb { get; set; }
}