using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ImageYoinker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(c => c.AddEnvironmentVariables("YOINKER_"))
    .ConfigureHostConfiguration(c => c.AddEnvironmentVariables("YOINKER_"))
    .Build<Startup>();

await host.RunAsync();