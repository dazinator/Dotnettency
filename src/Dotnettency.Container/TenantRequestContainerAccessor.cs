using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dotnettency.Container
{
    public class TenantRequestContainerAccessor<TTenant> : ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantContainerAccessor<TTenant> _tenantContainerAccessor;
        private readonly ILogger<TenantRequestContainerAccessor<TTenant>> _logger;

        public TenantRequestContainerAccessor(
            ILogger<TenantRequestContainerAccessor<TTenant>> logger,
            ITenantContainerAccessor<TTenant> tenantContainerAccessor)
        {
            _logger = logger;
            _tenantContainerAccessor = tenantContainerAccessor;

            TenantRequestContainer = new Lazy<Task<PerRequestContainer>>(async () =>
            {
                var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
                if (tenantContainer == null)
                {
                    return null;
                }

                var requestContainer = tenantContainer.CreateNestedContainer();
                return new PerRequestContainer(requestContainer);
            });
        }

        public Lazy<Task<PerRequestContainer>> TenantRequestContainer { get; private set; }
    }
}