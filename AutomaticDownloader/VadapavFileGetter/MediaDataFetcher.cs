using AutomaticDownloader.Http;
using AutomaticDownloader.VadapavFileGetter;
using HtmlAgilityPack;

namespace AutomaticDownloader.FileGetter
{
    public sealed class MediaDataFetcher(string url) : IGetMediaLink
    {
        private readonly string _url = url;
        public HashSet<string> _possibleFileFormats =
        [
            "mkv",
            "vlc",
            "mov",
            "flv",
            "mp4"
        ];

        private KeyValuePair<string, string> GetNameAndSeason(HtmlDocument body)
        {
            var dir = body.DocumentNode
                .SelectNodes("//div")
                .Where(node => node.Attributes["class"] is not null && node.Attributes["class"].Value == "directory").First()
                .SelectNodes("//span")
                .Where(node => !node.InnerText.Contains('/') && !node.InnerText.Contains("TV") && !node.InnerText.Contains("Movies"))
                .Select(node => node.InnerText);

            if (dir.Any())
            {
                return new KeyValuePair<string, string>(dir.First(), dir.Count() == 2 ? dir.Last() : "");
            }
            return new KeyValuePair<string, string>();
        }

        public MediaFileInfo GetMediaInfo()
        {
            var linkParsedDictionary = new Dictionary<string[], int>();
            var response = HttpHelper.GetBody(_url);
            var seriesNameAndSeason = GetNameAndSeason(response);
            foreach (var link in ParseBodyAndReturnRelativeLinks(response))
            {
                linkParsedDictionary[[link, link]] = 0;
            }

            return new MediaFileInfo
            {
                Title = seriesNameAndSeason.Key,
                Season = seriesNameAndSeason.Value,
                MediaLink = linkParsedDictionary
            };
        }

        private IEnumerable<string> ParseBodyAndReturnRelativeLinks(HtmlDocument body)
        {
            //There only will be 1 ul, and it will contain the link (FOR VADAPAV only)
            return body.DocumentNode
                .SelectNodes("//ul").First()
                .SelectNodes("//a")
                .Where(name => _possibleFileFormats.Contains(name.InnerText.Split(".").Last()))
                .Select(link => link.Attributes["href"])
                .Select(ATag => ATag.Value); 
        }
    }
}
