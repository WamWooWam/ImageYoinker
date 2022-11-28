using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ImageYoinker
{
    public class ImageYoinker
    {
        private IConfiguration _configuration;
        private ILoggerFactory _loggerFactory;
        private IHttpClientFactory _httpClientFactory;

        public ImageYoinker(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _httpClientFactory = httpClientFactory;
        }

        public async Task ProcessAsync(DiscordMessage message)
        {
            using var httpClient = _httpClientFactory.CreateClient("ImageYoinker");
            var yoinker = new ImageYoinkContext(
                _loggerFactory.CreateLogger<ImageYoinkContext>(),
                httpClient,
                _configuration["Folder"],
                $"{message.Id}");

            foreach (var embed in message.Embeds)
            {
                if (embed.Image != null)
                {
                    await yoinker.DownloadEmbedAsync(embed.Image.Url.ToUri());
                }

                if (embed.Thumbnail != null)
                {
                    await yoinker.DownloadEmbedAsync(embed.Thumbnail.Url.ToUri());
                }

                if (embed.Video != null)
                {
                    await yoinker.DownloadEmbedAsync(embed.Video.Url);
                }
            }

            foreach (var attachment in message.Attachments)
            {
                if (attachment.Width == null || attachment.Height == null) continue;

                await yoinker.DownloadAttachmentAsync(new Uri(attachment.Url));
            }
        }
    }
}
