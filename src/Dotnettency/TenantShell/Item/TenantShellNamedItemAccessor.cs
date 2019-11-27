using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellNamedItemAccessor<TTenant, TItem> : ITenantShellNamedItemAccessor<TTenant, TItem>
   where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly Dictionary<string, ITenantShellItemFactory<TTenant, TItem>> _namedFactories;
        
        public TenantShellNamedItemAccessor(
            ITenantShellAccessor<TTenant> tenantShellAccessor,
            Dictionary<string, ITenantShellItemFactory<TTenant, TItem>> namedFactories)
        {
            _tenantShellAccessor = tenantShellAccessor;
            _namedFactories = namedFactories;

            NamedFactory = new Func<IServiceProvider, string, Lazy<Task<TItem>>>((sp, name) =>
            {
                return new Lazy<Task<TItem>>(async () =>
                {
                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        throw new InvalidOperationException("No tenant shell was available to resolve a tenant shell item from. Type: " + typeof(TItem).Name);
                    }

                    Func<Lazy<Task<TItem>>> createLazyFactoryFunc = () =>
                    {
                        return new Lazy<Task<TItem>>(() =>
                        {
                            var tenant = tenantShell?.Tenant;
                            var fact = _namedFactories[name];                           
                            return fact?.Create(sp, tenant);
                        });
                    };

                    var tenantPipeline = tenantShell.GetOrAddItem(createLazyFactoryFunc, name);
                    return await tenantPipeline.Value;
                });
            });

        }
        
        public Func<IServiceProvider, string, Lazy<Task<TItem>>> NamedFactory { get; private set; }
    }

}
