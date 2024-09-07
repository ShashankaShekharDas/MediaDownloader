using AutomaticDownloader.Http;
using AutomaticDownloader.Interfaces;
using HtmlAgilityPack;

namespace AutomaticDownloader.VadapavFileGetter;

public sealed class MediaDataFetcher : IGetMediaLink
{
    private readonly HashSet<string> _possibleFileFormats =
    [
        "mkv",
        "vlc",
        "mov",
        "flv",
        "mp4",
        "m2ts",
        "avi"
    ];

    private readonly string _url;

    public MediaDataFetcher(string url)
    {
        _url = url;
    }

    public MediaFileInfo GetMediaInfo()
    {
        var linkParsedDictionary = new Dictionary<string[], int>();
        var response = HttpHelper.GetBody(_url);
        var seriesNameAndSeason = GetNameAndSeason(response);
        foreach (var link in ParseBodyAndReturnRelativeLinks(response)) linkParsedDictionary[[link[0], link[1]]] = 0;

        return new MediaFileInfo
        {
            Title = seriesNameAndSeason.Key,
            Season = seriesNameAndSeason.Value,
            MediaLink = linkParsedDictionary
        };
    }

    private static KeyValuePair<string, string> GetNameAndSeason(HtmlDocument body)
    {
        var dir = body.DocumentNode
            .SelectNodes("//div")
            .First(node => node.Attributes["class"] is not null && node.Attributes["class"].Value == "directory")
            .SelectNodes("//span")
            .Where(node => !node.InnerText.Contains('/') && !node.InnerText.Contains("TV") &&
                           !node.InnerText.Contains("Movies"))
            .Select(node => node.InnerText);

        if (dir.Any()) return new KeyValuePair<string, string>(dir.First(), dir.Count() == 2 ? dir.Last() : "");
        return new KeyValuePair<string, string>();
    }

    private IEnumerable<string[]> ParseBodyAndReturnRelativeLinks(HtmlDocument body)
    {
        //There only will be 1 ul, and it will contain the link (FOR VADAPAV only)
#pragma warning disable S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
        return body.DocumentNode
            .SelectNodes("//ul")[0]
            .SelectNodes("//a")
            .Where(name => _possibleFileFormats.Contains(name.InnerText.Split(".").Last()))
            .Select<HtmlNode, string[]>(link => [link.InnerText, link.Attributes["href"].Value]);
#pragma warning restore S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
    }
}