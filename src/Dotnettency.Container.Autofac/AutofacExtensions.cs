using Autofac;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dotnettency
{
    public static class AutofacExtensions
    {
        public static void AddDotnettencyContainerServices(this ContainerBuilder builder)
        {
            builder.RegisterType<AutofacTenantContainerAdaptor>()
                .As<ITenantContainerAdaptor>().InstancePerLifetimeScope()
                .As<IServiceProvider>().InstancePerLifetimeScope();
            //  builder.RegisterType<>
            //registry.For<ITenantContainerAdaptor>()
            //    .LifecycleIs(Lifecycles.Container)
            //    .Use<StructureMapTenantContainerAdaptor>();

            // registry.Forward<ITenantContainerAdaptor, IServiceProvider>();
            builder.RegisterType<TenantContainerServiceScopeFactory>()
                .As<IServiceScopeFactory>().InstancePerLifetimeScope();
                    

            //registry.For<IServiceScopeFactory>()
            //    .LifecycleIs(Lifecycles.Container)
            //    .Use<TenantContainerServiceScopeFactory>();
        }
    }
}
