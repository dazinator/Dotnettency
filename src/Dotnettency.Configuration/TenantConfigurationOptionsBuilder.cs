using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Configuration
{
    //public class TenantConfigurationOptionsBuilder<TTenant>
    //where TTenant : class
    //{

    //    public TenantConfigurationOptionsBuilder(
    //       MultitenancyOptionsBuilder<TTenant> builder)
    //    {
    //        Builder = builder;
    //    }

    //    public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }


    //    public MultitenancyOptionsBuilder<TTenant> ConfigureTenant(Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> configureTenantDelegate)
    //    {
    //        var factory = new DelegateTenantConfigurationFactory<TTenant>(configureTenantDelegate);
    //        Builder.Services.AddSingleton<ITenantConfigurationFactory<TTenant, IConfiguration>>(factory);
    //        Builder.Services.AddScoped<ITenantConfigurationAccessor<TTenant, IConfiguration>, TenantConfigurationAccessor<TTenant>>();

    //        //TODO: now where foes this factory get called to initialise the tenants config?
    //        // register the factory in app level services,
    //        // then at request time use the accessor to access IConfiguration
    //        return Builder;
    //    }
    //}
}
