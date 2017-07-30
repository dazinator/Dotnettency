using Dotnettency.Container;
using Dotnettency.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dotnettency
{
    public static class ModuleContainerBuilderOptionsExtensions
    {
        // WithModules() - TODO: simply bootstraps the tenants container with modules.


        /// <summary>
        /// Creates a nested container for each module to register its servies in - keeps isolation between modules.
        //                        and modules can't polute tenant level container.
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="options"></param>
        /// <param name="configureTenant"></param>
        /// <returns></returns>
        public static ContainerBuilderOptions<TTenant> WithModuleContainers<TTenant>(this AdaptedContainerBuilderOptions<TTenant> options)
        where TTenant : class
        {

            var wrappedFunc = options.TenantContainerAdaptor;

            options.TenantContainerAdaptor = new Func<ITenantContainerAdaptor>(() =>
            {
                var adaptor = wrappedFunc(); // builds the tenants container and gets it adaptor.

                // TODO: discover modules dynamically here. (i.e scanning, or some service).
                var modules = new List<IModule>();
                var moduleContainers = new List<Tuple<IModule, ITenantContainerAdaptor>>();

                // for each module create a nested container.
                foreach (var module in modules)
                {
                    var moduleContainer = adaptor.CreateChildContainer();
                    moduleContainer.Configure(module.ConfigureModule);
                    moduleContainers.Add(new Tuple<IModule, ITenantContainerAdaptor>(module, moduleContainer));
                }

                return adaptor;
            });

            return options.ContainerBuilderOptions;
        }
    }

}