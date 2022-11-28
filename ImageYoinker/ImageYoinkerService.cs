using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ImageYoinker
{
    public class ImageYoinkerService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly ImageYoinker _yoinker;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImageYoinkerService> _logger;

        private readonly ulong _channelId;

        public ImageYoinkerService(
            IConfiguration configuration,
            ILogger<ImageYoinkerService> logger,
            DiscordClient client,
            ImageYoinker yoinker)
        {
            _client = client;
            _configuration = configuration;
            _yoinker = yoinker;
            _channelId = configuration.GetValue<ulong>("ChannelId");
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageCreated += OnMessageCreated;
            _client.MessageUpdated += OnMessageUpdated;

            await _client.ConnectAsync();
        }
        
        private async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Channel.Id != _channelId && !e.Channel.IsPrivate)
                return;

            _logger.LogInformation("Processing message {MessageId}", e.Message.Id);

            await _yoinker.ProcessAsync(e.Message);
        }

        private async Task OnMessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            if (e.Channel.Id != _channelId && !e.Channel.IsPrivate)
                return;

            _logger.LogInformation("Reprocessing edited message {MessageId}", e.Message.Id);

            await _yoinker.ProcessAsync(e.Message);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }
    }
}
