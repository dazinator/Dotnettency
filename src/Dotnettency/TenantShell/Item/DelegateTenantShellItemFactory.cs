using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    /// <summary>
    /// A factory that uses a Func to create a <see cref="TItem"/>
    /// </summary>
    /// <typeparam name="TTenant"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class DelegateTenantShellItemFactory<TTenant, TItem> : ITenantShellItemFactory<TTenant, TItem>
        where TTenant : class
    {
        private readonly Func<TenantShellItemBuilderContext<TTenant>, TItem> _buildTenantItem;

        public DelegateTenantShellItemFactory(Func<TenantShellItemBuilderContext<TTenant>, TItem> buildTenantItem)
        {
            _buildTenantItem = buildTenantItem;
        }

        public async Task<TItem> Create(IServiceProvider serviceProviderOverride, TTenant tenant)
        {
            return await CreateTenantItem(serviceProviderOverride, tenant);
        }

        protected virtual Task<TItem> CreateTenantItem(IServiceProvider services, TTenant tenant)
        {
            return Task.Run(() =>
            {
                var builderContext = new TenantShellItemBuilderContext<TTenant>
                {
                    Tenant = tenant,
                    Services = services
                };
             
                var item =_buildTenantItem(builderContext);
                return item;
            });
        }
    }
}
