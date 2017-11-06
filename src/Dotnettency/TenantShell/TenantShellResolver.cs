﻿using System.Threading;
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

        public async Task<TenantShell<TTenant>> ResolveTenant(TenantDistinguisher identifier, ITenantShellFactory<TTenant> tenantFactory)
        {
            TenantShell<TTenant> result;
            if (_cache.TryGetValue(identifier, out result))
            {
                return result;
            }

            try
            {
                await _semaphore.WaitAsync();
                // Check again incase this is another thread that got queued at the semephor.
                if (_cache.TryGetValue(identifier, out result))
                {
                    return result;
                }

                var tenantResult = await tenantFactory.Get(identifier);
                if (tenantResult == null)
                {

                    // don't create null shell tenants - future requests with this identifier will still not find a shell for the tenant,
                    // so they will keep flowing through to ask the factory for the tenant. This is better if you are creating new tenants dynamically and a request is
                    // recieved with the tenants identifier, before the tenant has been set up in the source. It means the request will resolve to a null
                    // tenant for a while (factory may keep returning null), and eventually when a tenant is actually returned from the factory, a new tenant shell will then be created and added to 
                    // the cache, at which point the factory will stop being used for subsequent requests.
                    return null;
                }

                //if (CreateNullTenants)
                //{
                // create a shell for the null tenant, and add to cache. Future requests with same identifier
                // will yeild the cached shell for the null tenant.
                //var defaultTenantValue = default(TTenant);
                //var shellForNullTenant = new TenantShell<TTenant>(defaultTenantValue, identifier);
                //_mappings.AddOrUpdate(identifier, shellForNullTenant, (a, b) => { return shellForNullTenant; });
                //return shellForNullTenant;

                ////  }

                // We got a new shell tenant, so add it to the cache under its identifier, and any additional identifiers.
                _cache.AddOrUpdate(identifier, tenantResult, (a, b) => { return tenantResult; });
                bool distinguisherFound = false;
                foreach (var item in tenantResult.Distinguishers)
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
                    tenantResult.Distinguishers.Add(identifier);
                }

                return tenantResult;
                // todo
            }
            finally
            {
                _semaphore.Release();
            }

        }
    }
}
