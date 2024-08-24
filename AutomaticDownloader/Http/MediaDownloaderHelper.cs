namespace AutomaticDownloader.Http
{
    public sealed class MediaDownloaderHelper
    {
        private readonly HttpClient _httpClient;
        private string _baseDirectory = "./";
        private static MediaDownloaderHelper? _singleton = null;
        private readonly Dictionary<bool, List<MediaDownloaderHelper>> _availabiltyInstanceRecord = new()
        {
            [true] = [],
            [false] = []
        };

        private MediaDownloaderHelper()
        {
            _httpClient = new HttpClient();
        }

        public static MediaDownloaderHelper Init(int numberOfConcurrentDownloads, string baseDirectory)
        {
            if (_singleton != null)
            {
                return _singleton;
            }

            _singleton = new MediaDownloaderHelper();

            for (int i = 0; i < numberOfConcurrentDownloads; i++)
            {
                _singleton._availabiltyInstanceRecord[true].Add(new MediaDownloaderHelper() { _baseDirectory = baseDirectory });
            }

            return _singleton;
        }

        public MediaDownloaderHelper? GetConnection()
        {
#pragma warning disable S2551 // Shared resources should not be used for locking. Fix in the future
            lock (this)
            {
                if (_availabiltyInstanceRecord[true].Count != 0)
                {
                    MediaDownloaderHelper downloader = _availabiltyInstanceRecord[true][0];
                    _availabiltyInstanceRecord[true].Remove(downloader);
                    _availabiltyInstanceRecord[false].Add(downloader);
                    return downloader;
                }
            }
#pragma warning restore S2551 // Shared resources should not be used for locking
            return null;
        }

        public void FreeConnection(MediaDownloaderHelper mediaDownloader)
        {
            _availabiltyInstanceRecord[false].Remove(mediaDownloader);
            _availabiltyInstanceRecord[true].Add(mediaDownloader);
        }

        public async Task<bool> DownloadFile(string siteUrl, string relativeFilePath, string fileName)
        {
            Console.WriteLine($"Download started for file {fileName}");
            string downloadFileName = Path.Combine(_baseDirectory, fileName);
            string[] urlParts = siteUrl.Split('/');
            string filePath = string.Join("/", urlParts.Take(urlParts.Length - 2).Append(relativeFilePath[1..]));
            bool success = false;
            try
            {
                Console.WriteLine($"Download file: {filePath}");
                using Stream dataStream = await _httpClient.GetStreamAsync(filePath);
                using FileStream fileStream = new FileStream(downloadFileName, FileMode.OpenOrCreate);
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
