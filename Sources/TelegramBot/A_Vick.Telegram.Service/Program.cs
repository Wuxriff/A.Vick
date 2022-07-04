using A_Vick.Telegram.DataAccess.Extensions;
using A_Vick.Telegram.Models.Models;
using A_Vick.Telegram.Service.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace A_Vick.Telegram.Service
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("console"));

            var pathToExe = Environment.ProcessPath;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddServices();
                    services.AddDbContexts();
                    services.AddHostedServices();
                    services.AddOption<TelegramBotOptions>(hostContext.Configuration);
                })
                .UseContentRoot(pathToContentRoot);

            if (isService)
            {
                builder.Start();
            }
            else
            {
                Console.WriteLine("Press CTRL+C to stop the application");
                await builder.RunConsoleAsync();
            }
        }
    }
}