using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace NlogGenericHost
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var config = LogManager.Configuration;
            ////config.FindRuleByName("ruleInfo").SetLoggingLevels(NLog.LogLevel.Info, NLog.LogLevel.Debug);//.EnableLoggingForLevel(NLog.LogLevel.Trace); //enable level debug
            //config.FindRuleByName("ruleInfo").EnableLoggingForLevels(NLog.LogLevel.Info, NLog.LogLevel.Debug);


            config.RemoveRuleByName("ruleInfo"); // Remove rule
            //config.RemoveRuleByName("ruleDebug"); // Remove rule
            config.RemoveRuleByName("ruleWarn"); // Remove rule
            config.RemoveRuleByName("ruleTrace"); // Remove rule

            //LogManager.Configuration = config; // Apply new config

            //LogManager.GlobalThreshold = NLog.LogLevel.Debug;
            var logger = LogManager.GetCurrentClassLogger();
            var hostBuilder = new HostBuilder()
                .UseNLog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        loggingBuilder.AddNLog();
                    });

                    services.AddHostedService<ConsoleHostedService>();
                });

            await hostBuilder.RunConsoleAsync();
            Console.WriteLine("The host container has terminated. Press ANY key to exit the console.");
            Console.ReadKey();
        }
    }

    public class ConsoleHostedService : BackgroundService
    {
        private readonly ILogger<ConsoleHostedService> _logger;

        public ConsoleHostedService(ILogger<ConsoleHostedService> logger)
        {
            _logger = logger;
            _logger.LogInformation("ConsoleHostedService instance created...");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hello from your hosted service thread!");
            _logger.LogInformation("I may or may not return for a long time depending on what I do.");
            _logger.LogInformation("In this example, I return right away, but my host will continue to run until");
            _logger.LogInformation("its CancellationToken is Cancelled (SIGTERM(Ctrl-C) or a Lifetime Event )");
            _logger.LogWarning("Logging Warn!");
            _logger.LogDebug("Logging Debug!");
            _logger.LogTrace("Logging Trace!");
            await Task.CompletedTask;
        }
    }
}