using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dotnettency.Container
{
    public class TenantRequestContainerAccessor<TTenant> : ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly ITenantContainerFactory<TTenant> _containerFactory;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantContainerAccessor<TTenant> _tenantContainerAccessor;

        private readonly ILogger<TenantRequestContainerAccessor<TTenant>> _logger;

        public TenantRequestContainerAccessor(
            ILogger<TenantRequestContainerAccessor<TTenant>> logger,
            IHttpContextAccessor httpContextAccessor,
            ITenantContainerAccessor<TTenant> tenantContainerAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tenantContainerAccessor = tenantContainerAccessor;

            TenantRequestContainer = new Lazy<Task<PerRequestContainer>>(async () =>
            {
                var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
                if (tenantContainer == null)
                {
                    return null;
                }

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return null;
                }

                var requestContainer = tenantContainer.CreateNestedContainer();
                var tenantRequestContainer = new PerRequestContainer(requestContainer, httpContext);
                return tenantRequestContainer;
            });
        }

        public Lazy<Task<PerRequestContainer>> TenantRequestContainer { get; private set; }

    }
}