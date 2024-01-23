using System;
using Dotnettency.Container;
using Dotnettency.Container.Native;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dotnettency;

public static class ChildContainersExtensions
{
    // /// <summary>
    // /// Should be added to the root application container. Allows a <see cref="ITenantContainerAdaptor"/> to be injected in the application, and used to create child contiainers.
    // /// </summary>
    // /// <param name="serviceCollection"></param>
    // /// <returns></returns>
    // public static IServiceCollection AddChildContainers(this IServiceCollection serviceCollection)
    // {
    //     serviceCollection.AddSingleton<ITenantContainerAdaptor>(sp =>
    //     {
    //         var logger = sp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>();
    //         var adaptor = new NativeTenantContainerAdaptor(logger, sp, serviceCollection, ContainerRole.Root, "Root");
    //         return adaptor;
    //     });
    //     //serviceCollection.AddScoped<IServiceScopeFactory, ChildContainerServiceScopeFactory>();
    //     return serviceCollection;
    // }

    // public class ChildContainerServiceScopeFactory : IServiceScopeFactory
    // {
    //     private readonly ITenantContainerAdaptor _containerAdaptor;
    //
    //     public ChildContainerServiceScopeFactory(ITenantContainerAdaptor containerAdaptor)
    //     {
    //         _containerAdaptor = containerAdaptor;
    //     }
    //     public IServiceScope CreateScope()
    //     {
    //         return _containerAdaptor.CreateScope();
    //     }
    // }
}