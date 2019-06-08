using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellResolver<TTenant> : ITenantShellResolver<TTenant>
        where TTenant : class
    {
        private readonly ITenantShellCache<TTenant> _cache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public TenantShellResolver(ITenantShellCache<TTenant> tenantShellCache)
        {
            _cache = tenantShellCache;
        }

        public async Task<TenantShell<TTenant>> ResolveTenantShell(TenantIdentifier identifier, ITenantShellFactory<TTenant> tenantFactory)
        {
            if (_cache.TryGetValue(identifier, out TenantShell<TTenant> result))
            {
                return result;
            }

            try
            {
                await _semaphore.WaitAsync();

                // Double locking - check cache again incase another thread just put it there.
                if (_cache.TryGetValue(identifier, out result))
                {
                    return result;
                }

                // initialise tenant shell for tenant identifier.
                var tenantResult = await tenantFactory.Get(identifier);
                if (tenantResult == null)
                {
                    // don't cache / associate null tenant shells with tenant identifiers. This means future requests for a tenant who currenlty has a null shell will contiue to flow throw to the factory 
                    // until it returns a tenant shell result.

                    // This is better flow if you are creating new tenants dynamically and a request is recieved on the new tenants URL before the tenants has been set up in the source.
                    // It means the request will resolve to a null tenant shell for a while, (factory may keep returning null), and eventually when a tenant is actually returned from the factory, a new tenant shell will then be created and added to 
                    // the cache, at which point the factory will stop being used for subsequent requests.
                    return null;
                }

                // We got a new shell, so add it to the cache under its identifier, and any additional identifiers.
                _cache.AddOrUpdate(identifier, tenantResult, (a, b) => { return tenantResult; });

                bool distinguisherFound = false;
                foreach (var item in tenantResult.Identifiers)
                {
                    if (item.Equals(identifier))
                    {
                        distinguisherFound = true;
                        continue;
                    }
                    _cache.AddOrUpdate(item, tenantResult, (a, b) => { return tenantResult; });
                }

                if (!distinguisherFound)
                {
                    tenantResult.Identifiers.Add(identifier);
                }

                return tenantResult;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IDisposable> RemoveTenantShell(TenantIdentifier identifier)
        {

            TenantShell<TTenant> removed = null;

            if (!_cache.TryGetValue(identifier, out TenantShell<TTenant> result))
            {
                // tenant hasn't yet been initialised.. so nothing to do..
                return null;
            }

            try
            {
                await _semaphore.WaitAsync(); // block restarting if a tenant intiialise in process or another restart in prcess.

                // tenant may have just been restarted on another thread and removed from cache, so check again.
                //Todo: make this more robust if TryRemove returns false for example..
                var isRemoved = _cache.TryRemove(identifier, out removed);
                if (isRemoved)
                {
                    foreach (var item in removed.Identifiers)
                    {
                        if (item.Equals(identifier))
                        {
                            continue;
                        }
                        isRemoved = _cache.TryRemove(item, out TenantShell<TTenant> itemRemoved);
                    }
                }

                // dispose after small delay to allow respoinse to be returned flowing back through the same pipeline.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                return removed;
                //Task.Delay(new TimeSpan(0, 0, 2)).ContinueWith((t) =>
                //{
                //    removed?.Dispose();
                //});
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
