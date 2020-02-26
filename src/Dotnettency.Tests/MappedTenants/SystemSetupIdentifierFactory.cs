using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Dotnettency.Tests
{
    public class SystemSetupIdentifierFactory : MappedHttpContextTenantIdentifierFactory<Tenant, int>
    {

        private readonly IOptionsMonitor<SystemSetupOptions> _systemSetupOptionsMonitor;

        private static Task<TenantIdentifier> _systemSetupTenantId = Task.FromResult(CreateIdentifier(-1));

        public SystemSetupIdentifierFactory(
            ILogger<MappedHttpContextTenantIdentifierFactory<Tenant, int>> logger,
            IHttpContextProvider httpContextAccessor,
            IHttpContextValueSelector valueSelector,
            IOptionsProvider<TenantMappingOptions<int>> optionsMonitor,
            ITenantMatcherFactory<int> matcherFactory,
            IOptionsMonitor<SystemSetupOptions> systemSetupOptionsMonitor) : base(logger, httpContextAccessor, valueSelector, optionsMonitor, matcherFactory)
        {
            _systemSetupOptionsMonitor = systemSetupOptionsMonitor;
        }

        public override Task<TenantIdentifier> IdentifyTenant()
        {
            if(_systemSetupOptionsMonitor.CurrentValue.IsSystemSetupComplete)
            {
                return base.IdentifyTenant();
            }
            else
            {
                return _systemSetupTenantId;
            }
        }

    }
}




