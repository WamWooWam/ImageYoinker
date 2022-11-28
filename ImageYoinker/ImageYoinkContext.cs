using Microsoft.Extensions.Logging;

namespace ImageYoinker
{
    public class ImageYoinkContext
    {
        private ILogger<ImageYoinkContext> _logger;
        private HttpClient _httpClient;
        private string _folderPath;
        private string _baseName;

        public ImageYoinkContext(
            ILogger<ImageYoinkContext> logger,
            HttpClient httpClient,
            string folderPath,
            string baseName)
        {
            _httpClient = httpClient;
            _folderPath = folderPath;
            _baseName = baseName;
            _logger = logger;
        }

        public async Task DownloadEmbedAsync(Uri url)
        {
            if (url.Host == "preview.redd.it")
                url = new UriBuilder(url) { Host = "i.redd.it", Query = "" }.Uri;

            var filename = Path.GetFileName(url.AbsolutePath);
            if (Path.GetFileNameWithoutExtension(filename).ToLowerInvariant() == "unknown")
                filename = $"{_baseName}-{filename}";

            var filePath = Path.Combine(_folderPath, filename);
            await DownloadToFileAsync(url, filePath);
        }

        public async Task DownloadAttachmentAsync(Uri url)
        {
            var filename = Path.GetFileName(url.AbsolutePath);
            var filePath = Path.Combine(_folderPath, $"{_baseName}-{filename}");
            await DownloadToFileAsync(url, filePath);
        }

        private async Task DownloadToFileAsync(Uri url, string filePath)
        {
            try
            {
                using var file = File.Create(filePath, 8 * 1024, FileOptions.Asynchronous);
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                using var responseStream = await response.Content.ReadAsStreamAsync();

                await responseStream.CopyToAsync(file);
            }
            // if the file already exists it'll fail with this error
            catch (IOException ex) when (ex.HResult == -2147024816 || ex.HResult == -2147024713)
            {
                _logger.LogInformation(ex, "File \"{Path}\" already exists, ignoring...", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File download failed for {Url}", url);
            }
        }
    }
}
