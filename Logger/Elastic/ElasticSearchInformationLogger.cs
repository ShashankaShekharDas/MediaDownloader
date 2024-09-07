using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Logger.Interfaces;
using Logger.LoggerType;

namespace Logger.Elastic;

public class ElasticSearchInformationLogger : IInformationLogger
{
    private readonly ElasticsearchClient _client;
    private readonly string _host = Environment.GetEnvironmentVariable("elasticHost") ?? "https://100.97.131.25:9200/";
    private readonly string _index = $"media-downloader-{DateTime.Today:yyyy-MM-dd}";

    public ElasticSearchInformationLogger()
    {
        var settings = new ElasticsearchClientSettings(new Uri(_host))
            .Authentication(new BasicAuthentication("elastic", "xxxxx"))
            .ServerCertificateValidationCallback((ob, cert, chain, errors) => true);

        _client = new ElasticsearchClient(settings);
    }

    public async Task<bool> AsyncLog(ElasticLogRecord message)
    {
        var response = await _client.IndexAsync(message, index => index.Index(_index));

        Console.WriteLine(response.IsValidResponse
            ? "Document indexed successfully."
            : $"Failed to index document: {response.DebugInformation}");

        return response.IsValidResponse;
    }
}