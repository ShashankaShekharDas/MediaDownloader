using Microsoft.Extensions.Logging;

namespace Logger;

public static class GetConsoleLogger
{
    private static readonly ILoggerFactory _loggerFactory;

    static GetConsoleLogger()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole(); // Add more providers as needed
            builder.SetMinimumLevel(LogLevel.Information);
        });
    }
    
    public static ILogger GetLogger(string projectName)
    {
        return _loggerFactory.CreateLogger(projectName);
    }
}