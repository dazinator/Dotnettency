using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantHostedServiceManager<TTenant> : IDisposable
         where TTenant : class
    {
        // private readonly TenantShellItemBuilderContext<TTenant> _context;
        private readonly ILogger<TenantHostedServiceManager<TTenant>> _logger;

        public TimeSpan StoppingTimeout { get; set; } = new TimeSpan(0, 0, 5);

       
        public List<IHostedService> HostedServices { get; set; }

        public TenantHostedServiceManager(ILogger<TenantHostedServiceManager<TTenant>> logger, IEnumerable<IHostedService> hostedServices)
        {
            _logger = logger;
            HostedServices = Sanitise(hostedServices).ToList();
        }

        public TenantHostedServiceManager<TTenant> Remove<T>()
           where T : IHostedService
        {
            HostedServices.RemoveAll(a => a.GetType() == typeof(T));
            return this;
        }

        private IEnumerable<IHostedService> Sanitise(IEnumerable<IHostedService> hostedServices)
        {
            // The GenericWebHostService is a host level background service and can only be started as a singleton.
            return hostedServices.Where(a => a.GetType().Name != "GenericWebHostService");
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting hosted services for tenant.");

            foreach (var item in HostedServices)
            {
                await item.StartAsync(CancellationToken.None).ConfigureAwait(false);
            }

            _logger.LogInformation("Hosted services started for tenant.");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stopping tenant hosted services");

            using (var cts = new CancellationTokenSource(StoppingTimeout))
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
            {
                var token = linkedCts.Token;
                //// Trigger IApplicationLifetime.ApplicationStopping
                //_applicationLifetime?.StopApplication();

                IList<Exception> exceptions = new List<Exception>();
                if (HostedServices != null) // Started?
                {
                    HostedServices.Reverse();
                    foreach (var hostedService in HostedServices)
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            await hostedService.StopAsync(token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }

                token.ThrowIfCancellationRequested();
                // await _hostLifetime.StopAsync(token);

                // Fire IApplicationLifetime.Stopped
                // _applicationLifetime?.NotifyStopped();

                if (exceptions.Count > 0)
                {
                    var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
                    _logger.LogError(ex, ex.Message);
                    throw ex;
                }
            }

            _logger.LogInformation("Tenant hosted services were stopped.");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    try
                    {
                        StopAsync(default(CancellationToken)).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "error on dispose");
                        throw;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TenantHostedServiceManager()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



    }
}
