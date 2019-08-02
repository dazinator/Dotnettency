using System;

namespace Dotnettency.Configuration
{
    public class TenantConfigurationBuilderContext<TTenant>
    {
        public TTenant Tenant { get; set; }

        public IServiceProvider ApplicationServices { get; set; }
    }
}
