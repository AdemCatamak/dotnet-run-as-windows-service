using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Services
{
    public class DummyHostedService : IHostedService
    {
        private readonly ILogger<DummyHostedService> _logger;

        public DummyHostedService(ILogger<DummyHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
           _logger.LogInformation($"{nameof(DummyHostedService)} started");
           return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(DummyHostedService)} stopped");
            return Task.CompletedTask;
        }
    }
}