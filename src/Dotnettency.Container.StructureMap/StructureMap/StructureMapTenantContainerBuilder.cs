using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    //public class StructureMapTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    //{
    //    public StructureMapTenantContainerBuilder(IContainer container, Action<TTenant, ConfigurationExpression> configure)
    //    {
    //        // Ensure.Argument.NotNull(container, nameof(container));
    //        // Ensure.Argument.NotNull(configure, nameof(configure));

    //        Container = container;
    //        Configure = configure;
    //    }

    //    protected IContainer Container { get; }
    //    protected Action<TTenant, ConfigurationExpression> Configure { get; }

    //    public virtual Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
    //    {
    //        // Ensure.Argument.NotNull(tenant, nameof(tenant));

    //        var tenantContainer = Container.CreateChildContainer();

            


    //        tenantContainer.Configure(config =>
    //        {
    //           // config.For<IServiceScopeFactory>()
    //           //.LifecycleIs(Lifecycles.Container)
    //           //.Use<TenantContainerServiceScopeFactory>();
    //            Configure(tenant, config);
    //        });
    //        var report = tenantContainer.WhatDoIHave();
    //        ITenantContainerAdaptor adaptor = tenantContainer.GetInstance<ITenantContainerAdaptor>();

    //            //new StructureMapTenantContainerAdaptor(tenantContainer, ContainerRole.Child);

    //       // IServiceScopeFactory

    //        // var sp = adaptor.GetServiceProvider();
          


    //        return Task.FromResult(adaptor);
    //    }
    //}


}