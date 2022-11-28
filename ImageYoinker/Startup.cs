using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ImageYoinker
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public class Startup : IStartup
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _configuration = config;
            _loggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("ImageYoinker");

            services.AddSingleton<ImageYoinker>();
            services.AddHostedService<ImageYoinkerService>();

            var discordConfig = new DiscordConfiguration()
            {
                Token = _configuration["Token"],
                LoggerFactory = _loggerFactory,
                Intents = DiscordIntents.All
            };
            var discord = new DiscordClient(discordConfig);
            services.AddSingleton(discord);
        }

        public void Configure(IHostBuilder host)
        {

        }
    }
}
