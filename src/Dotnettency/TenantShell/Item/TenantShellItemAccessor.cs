using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellItemAccessor<TTenant, TItem> : ITenantShellItemAccessor<TTenant, TItem>
     where TTenant : class
    {
        private readonly ITenantShellItemFactory<TTenant, TItem> _tenantItemFactory;

        public TenantShellItemAccessor(ITenantShellItemFactory<TTenant, TItem> tenantItemFactory)
        {           
            _tenantItemFactory = tenantItemFactory;

            Factory = new Func<IServiceProvider, Lazy<Task<TItem>>>((sp) =>
            {
                return new Lazy<Task<TItem>>(async () =>
                {
                    var tenantShellAccessor = sp.GetRequiredService<ITenantShellAccessor<TTenant>>();
                    var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        throw new InvalidOperationException("No tenant shell was available to resolve a tenant shell item from. Type: " + typeof(TItem).Name);
                    }

                    Func<Lazy<Task<TItem>>> createLazyFactoryFunc = () =>
                    {
                        return new Lazy<Task<TItem>>(() =>
                        {
                            var tenant = tenantShell?.Tenant;
                            return _tenantItemFactory?.Create(sp, tenant);
                        });
                    };

                    var tenantPipeline = tenantShell.GetOrAddItem(createLazyFactoryFunc);
                    return await tenantPipeline.Value;
                });
            });

        }

        public Func<IServiceProvider, Lazy<Task<TItem>>> Factory { get; private set; }

    }

}
