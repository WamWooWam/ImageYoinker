using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ImageYoinker
{
    public static class Extensions
    {        
        public static IHost Build<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStartup>(this IHostBuilder builder) where TStartup : class, IStartup
        {
            builder.ConfigureServices((c, s) =>
            {
                var startup = ActivatorUtilities.CreateInstance<TStartup>(s.BuildServiceProvider());
                startup.Configure(builder);
                startup.ConfigureServices(s);
            });

            return builder.Build();
        }
    }

    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);

        void Configure(IHostBuilder host);
    }
}
