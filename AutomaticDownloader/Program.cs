﻿using AutomaticDownloader.Listeners;
using Logger;
using Microsoft.Extensions.Logging;

namespace AutomaticDownloader;

internal static class Program
{
    internal static ILogger Logger { get; set; } = GetConsoleLogger.GetLogger(AppDomain.CurrentDomain.FriendlyName);
    private static string Topic { get; set; } = Environment.GetEnvironmentVariable("topic") ?? "prd.mediadownloader.lab";
    
    private static int ConcurrentDownloads { get; set; } = Convert.ToInt32(Environment.GetEnvironmentVariable("concurrentdownloads") ?? "1");
    
    internal static void Main()
    {
        Logger.LogInformation($"Starting Downloader. Trying to listen to topic {Topic}");
        new KafkaMessageListener(Topic, ConcurrentDownloads).ListenMessage();
    }
}