namespace AutomaticDownloader.Http
{
    public sealed class MediaDownloader
    {
        private readonly HttpClient _httpClient;
        private string _baseDirectory = "./";
        private static MediaDownloader? _singleton = null;
        private Dictionary<bool, List<MediaDownloader>> _availabiltyInstanceRecord = new()
        {
            [true] = [],
            [false] = []
        };

        private MediaDownloader()
        {
            _httpClient = new();
        }

        public static MediaDownloader Init(int numberOfConcurrentDownloads, string baseDirectory)
        {
            if (_singleton != null)
            {
                return _singleton;
            }

            _singleton = new MediaDownloader();

            for (int i = 0; i < numberOfConcurrentDownloads; i++)
            {
                _singleton._availabiltyInstanceRecord[true].Add(new MediaDownloader() { _baseDirectory = baseDirectory});
            }

            return _singleton;
        }

        public MediaDownloader? GetConnection()
        {
            if (_availabiltyInstanceRecord[true].Count != 0)
            {
                var downloader = _availabiltyInstanceRecord[true].First();
                _availabiltyInstanceRecord[true].Remove(downloader);
                _availabiltyInstanceRecord[false].Add(downloader);
                return downloader;
            }
            return null;
        }

        private void FreeConnection(MediaDownloader mediaDownloader)
        {
            _availabiltyInstanceRecord[false].Remove(mediaDownloader);
            _availabiltyInstanceRecord[true].Add(mediaDownloader);
        }

        public async Task<bool> DownloadFile(string siteUrl, string relativeFilePath, string fileName)
        {
            // FileName should contain extension
            var urlParts = siteUrl.Split('/');
            var filePath = string.Join("/", urlParts.Take(urlParts.Length - 2).Append(relativeFilePath[1..]));
            try
            {
                using var dataStream = await _httpClient.GetStreamAsync(filePath);
                using var fileStream = new FileStream(Path.Combine(_baseDirectory, fileName), FileMode.OpenOrCreate);
                await dataStream.CopyToAsync(fileStream);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            FreeConnection(this);
            return false;
        }
    }
}
