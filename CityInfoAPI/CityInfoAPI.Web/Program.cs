using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace CityInfoAPI.Web
{
    #pragma warning disable CS1591

    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()

                // https://johan.driessen.se/posts/Properly-configuring-NLog-and-ILogger-in-ASP-NET-Core-2-2/
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    //Read configuration from appsettings.json
                    config
                        .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    //Add environment variables to config
                    config.AddEnvironmentVariables();

                    //Read NLog configuration from the nlog config file
                    //env.ConfigureNLog($"nlog.{env.EnvironmentName}.config");
                    env.ConfigureNLog($"nlog.config");
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                    logging.AddConsole();
                    logging.AddNLog();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                });

    }

    #pragma warning restore CS1591
}
