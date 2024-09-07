using HtmlAgilityPack;

namespace AutomaticDownloader.Http;

public static class HttpHelper
{
    private static readonly HtmlWeb _htmlWeb = new();

    public static HtmlDocument GetBody(string uri)
    {
        return _htmlWeb.Load(uri);
    }
}