using System;
using System.IO;
using System.Reflection;
using AspNetCore.Hosting.WindowsService;
using ConsoleApp1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace ConsoleApp1
{
    class Program
    {
        private static IConfigurationRoot _configuration;

        private static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location));

            ILogger<Program> logger = LoggerFactory.Create(builder => builder.AddConsole(options => { }))
                                                   .CreateLogger<Program>();
            try
            {
                logger.LogInformation("Host creation is starting");
                using (IHost host = CreateHost())
                {
                    var runAsWinService = _configuration.GetValue<bool>("RunAsWinService");

                    if (runAsWinService)
                    {
                        logger.LogInformation("Host will be start as service");
                        host.RunAsWinService();
                    }
                    else
                    {
                        logger.LogInformation("Host will be start as console");
                        host.Run();
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Fatal error occurs during main method's execution");
            }
            finally
            {
                logger.LogInformation("App is shutting down");
            }
        }
        
        private static IHost CreateHost()
        {
            return new HostBuilder()
                  .ConfigureAppConfiguration((context, builder) =>
                                             {
                                                 BuildApplicationConfiguration(builder);
                                                 _configuration = builder.Build();
                                             })
                  .ConfigureServices((hostContext, services) =>
                                     {
                                         services.Configure<HostOptions>(option => { option.ShutdownTimeout = TimeSpan.FromSeconds(value: 20); });
                                         InjectDependencies(services);
                                     })
                  .ConfigureLogging((host, logging) =>
                                    {
                                        logging.SetMinimumLevel(LogLevel.Information);
                                        logging.ClearProviders();
                                        logging.AddConsole();
                                    })
                  .Build();
        }

        private static void BuildApplicationConfiguration(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables();
        }

        private static void InjectDependencies(IServiceCollection services)
        {
            services.AddHostedService<DummyHostedService>();
        }
    }
}