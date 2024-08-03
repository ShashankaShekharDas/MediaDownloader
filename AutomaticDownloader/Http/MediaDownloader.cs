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
            _httpClient = new HttpClient();
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
            lock (this)
            {
                //Console.WriteLine($"Call made to GetConnection(), number of connections unused {_availabiltyInstanceRecord[true].Count} and used {_availabiltyInstanceRecord[false].Count}");
                if (_availabiltyInstanceRecord[true].Count != 0)
                {
                    var downloader = _availabiltyInstanceRecord[true].First();
                    _availabiltyInstanceRecord[true].Remove(downloader);
                    _availabiltyInstanceRecord[false].Add(downloader);
                    return downloader;
                }
            }
            return null;
        }

        public void FreeConnection(MediaDownloader mediaDownloader)
        {
            _availabiltyInstanceRecord[false].Remove(mediaDownloader);
            _availabiltyInstanceRecord[true].Add(mediaDownloader);
        }

        public async Task<bool> DownloadFile(string siteUrl, string relativeFilePath, string fileName)
        {
            Console.WriteLine($"Download started for file {fileName}");
            var downloadFileName = Path.Combine(_baseDirectory, fileName);
            var urlParts = siteUrl.Split('/');
            var filePath = string.Join("/", urlParts.Take(urlParts.Length - 2).Append(relativeFilePath[1..]));
            var success = false;
            try
            {
                Console.WriteLine($"Download file: {filePath}");
                using var dataStream = await _httpClient.GetStreamAsync(filePath);
                using var fileStream = new FileStream(downloadFileName, FileMode.OpenOrCreate);
                await dataStream.CopyToAsync(fileStream);
                success = true;
            }
            catch (Exception ex)
            {
                if (File.Exists(downloadFileName))
                {
                    Console.WriteLine("Deleting failed download file");
                    File.Delete(downloadFileName);
                }
                Console.WriteLine(ex.Message, ex.StackTrace);
            }

            return success;
        }
    }
}
