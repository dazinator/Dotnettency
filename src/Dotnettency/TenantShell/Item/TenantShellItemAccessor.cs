using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellItemAccessor<TTenant, TItem> : ITenantShellItemAccessor<TTenant, TItem>
     where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly ITenantShellItemFactory<TTenant, TItem> _tenantItemFactory;


        public TenantShellItemAccessor(ITenantShellAccessor<TTenant> tenantShellAccessor, ITenantShellItemFactory<TTenant, TItem> tenantItemFactory)
        {
            _tenantShellAccessor = tenantShellAccessor;
            _tenantItemFactory = tenantItemFactory;

            Factory = new Func<IServiceProvider, Lazy<Task<TItem>>>((sp) =>
            {
                return new Lazy<Task<TItem>>(async () =>
                {
                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        // no tenant shell - return application level service.
                        return (TItem)sp.GetService(typeof(TItem));
                    }

                    Func<Lazy<Task<TItem>>> createLazyFactoryFunc = () => {
                        return new Lazy<Task<TItem>>(() =>
                        {
                            var tenant = tenantShell?.Tenant;
                            return _tenantItemFactory.Create(sp, tenant);
                        });
                    };

                   // var factFunc = )
                    var tenantPipeline = tenantShell.GetOrAddItem(createLazyFactoryFunc);

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IServiceProvider, Lazy<Task<TItem>>> Factory { get; private set; }
    }

}
