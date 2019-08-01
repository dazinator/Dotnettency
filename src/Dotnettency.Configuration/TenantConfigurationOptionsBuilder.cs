using Microsoft.Extensions.Configuration;
using System;

namespace Dotnettency.Configuration
{
    public class TenantConfigurationOptionsBuilder<TTenant>
    where TTenant : class
    {    

        public TenantConfigurationOptionsBuilder(
           MultitenancyOptionsBuilder<TTenant> builder)
        {
            Builder = builder;
        }

        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }


        public MultitenancyOptionsBuilder<TTenant> ConfigureTenant(
     Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> configureTenantDelegate)
        {
            var factory = new DelegateTenantConfigurationFactory<TTenant>(configureTenantDelegate);
            //TODO: now where foes this factory get called to initialise the tenants config?
            return Builder;
        }
    }
}
