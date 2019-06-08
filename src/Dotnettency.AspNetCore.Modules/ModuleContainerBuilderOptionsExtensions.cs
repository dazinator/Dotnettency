using Dotnettency.Container;
using Dotnettency.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dotnettency
{

}

public class ModuleOptionsBuilder<TTenant>
     where TTenant : class
     where TModule : IModule
{

    private readonly AdaptedContainerBuilderOptions<TTenant> _parentOptions;

    public ModuleOptionsBuilder(AdaptedContainerBuilderOptions<TTenant> parentOptions)
    {
        _parentOptions = parentOptions;
    }

    public ModuleOptionsBuilder<TTenant> ActivateModules<TModule>(Func<ITenantContainerAdaptor, TTenant, IEnumerable<TModule>> factoryDelegate)
        where TModule : IModule
    {
        var factory = new DelegateModuleFactory<TTenant, TModule>(factoryDelegate);
        // Add a service to the host container that can be used to get modules for a tenant.
        _parentOptions.ContainerBuilderOptions.Builder.Services.AddSingleton<IModuleFactory<TTenant, TModule>>(factory);
        return this;
    }

    /// <summary>
    /// Allows you to configure additonal options for the shell for modules.
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ModuleOptionsBuilder<TTenant> ConfigureModuleShellOptions<TModule>(Action<ModuleShellOptionsBuilder<TTenant, TModule>> configure)
         where TModule : IModule
    {

        var factory = new DelegateModuleShellFactory<TTenant, TModule>((t, m) =>
        {
            var builder = new ModuleShellOptionsBuilder<TTenant, TModule>(t, m);
            configure(builder);
            var moduleShellOptions = builder.Build();
            return new ModuleShell<TModule>(m, moduleShellOptions);
        });

        // Add a service to the host container that can be used get the ModuleShell for a tenant's module.
        _parentOptions.ContainerBuilderOptions.Builder.Services.AddSingleton<IModuleShellFactory<TTenant, TModule>>(factory);

        return this;
    }

    ///// <summary>
    ///// Creates a nested container for each module to register its servies in - keeps isolation between modules.
    ////                        and modules can't polute tenant level container.
    ///// </summary>
    ///// <typeparam name="TTenant"></typeparam>
    ///// <param name="options"></param>
    ///// <param name="configureTenant"></param>
    ///// <returns></returns>
    //public static ModuleOptionsBuilder<TTenant, TModule> OnConfigureModuleShell(Action<ModuleShellOptionsBuilder<TTenant, TModule>> configure)
    //{

    //    var builder = new ModuleShellOptionsBuilder<TTenant, TModule>();
    //    configure(builder);
    //    return this;

    //    var wrappedFunc = options.HostContainerAdaptor;


    //    options.HostContainerAdaptor = new Func<ITenantContainerAdaptor>(() =>
    //    {
    //        var adaptor = wrappedFunc(); // builds the tenants container and gets it adaptor.

    //        // add another service to the container that can be called by middleware to intialise our modules.
    //        adaptor.Configure((services) =>
    //        {
    //            services.AddSingleton<>




    //            });

    //        // TODO: discover modules dynamically here. (i.e scanning, or some service).
    //        var modules = new List<IModule>();
    //        var moduleContainers = new List<Tuple<IModule, ITenantContainerAdaptor>>();

    //        // for each module create a nested container.
    //        foreach (var module in modules)
    //        {
    //            var moduleContainer = adaptor.CreateChildContainer();
    //            moduleContainer.Configure(module.ConfigureModule);
    //            moduleContainers.Add(new Tuple<IModule, ITenantContainerAdaptor>(module, moduleContainer));
    //        }

    //        return adaptor;
    //    });

    //    return options.ContainerBuilderOptions;
    //}


}

public static class ModuleContainerBuilderOptionsExtensions
{

    public static ContainerBuilderOptions<TTenant> HasModules<TTenant>(this AdaptedContainerBuilderOptions<TTenant> options, Action<ModuleOptionsBuilder<TTenant>> configure)
 where TTenant : class
        //    where TModule : IModule
    {
        var builder = new ModuleOptionsBuilder<TTenant>(options);
        configure(builder);
        return options.ContainerBuilderOptions;
    }
    // WithModules() - TODO: simply bootstraps the tenants container with modules.



}

}