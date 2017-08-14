using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    //public class TenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    //{
    //    private readonly ITenantContainerAdaptor _parentContainer;
    //    private readonly Action<TTenant, IServiceCollection> _configureTenant;

    //    public TenantContainerBuilder(ITenantContainerAdaptor parentContainer, Action<TTenant, IServiceCollection> configureTenant)
    //    {
    //        _parentContainer = parentContainer;
    //        _configureTenant = configureTenant;
    //    }

    //    public Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
    //    {

    //        var tenantContainer = _parentContainer.CreateChildContainer();
    //        tenantContainer.Configure(config =>
    //        {
    //            // config.For<IServiceScopeFactory>()
    //            //.LifecycleIs(Lifecycles.Container)
    //            //.Use<TenantContainerServiceScopeFactory>();
    //            _configureTenant(tenant, config);
    //        });
    //        //var report = tenantContainer.WhatDoIHave();
    //       // ITenantContainerAdaptor adaptor = tenantContainer.GetInstance<ITenantContainerAdaptor>();

    //        //new StructureMapTenantContainerAdaptor(tenantContainer, ContainerRole.Child);

    //        // IServiceScopeFactory

    //        // var sp = adaptor.GetServiceProvider();
    //        return Task.FromResult(tenantContainer);
    //    }
    //}


}