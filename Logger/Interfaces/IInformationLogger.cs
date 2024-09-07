using Logger.LoggerType;

namespace Logger.Interfaces;

public interface IInformationLogger
{
    public Task<bool> AsyncLog(ElasticLogRecord message);
}