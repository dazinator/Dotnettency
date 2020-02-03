using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Sample.Pages
{
    public class TimedTenantHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedTenantHostedService> _logger;
        private Timer _timer;
        private Task<Tenant> _currentTenant;

        public TimedTenantHostedService(ILogger<TimedTenantHostedService> logger, Task<Tenant> tenant)
        {
            _logger = logger;
            _currentTenant = tenant;
        }

        public Tenant CurrentTenant { get; set; }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            CurrentTenant = await _currentTenant;

            _logger.LogInformation($"Timed Hosted Service running for tenant: {CurrentTenant?.Name} ?? NULL");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}, Tenant: {TenantName}", count, CurrentTenant?.Name ?? "NULL");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
