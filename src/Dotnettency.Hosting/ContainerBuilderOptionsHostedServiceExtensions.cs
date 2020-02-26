using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;

namespace Dotnettency
{
    public static class ContainerBuilderOptionsHostedServiceExtensions
    {
        /// <summary>
        /// Starts any <see cref="IHostedService"s/> running once the tenant's container has been initalised.        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        /// 
        public static ContainerBuilderOptions<TTenant> UseTenantHostedServices<TTenant>(this ContainerBuilderOptions<TTenant> optionsBuilder, Action<TenantHostedServiceManager<TTenant>> configure = null, CancellationToken cancellationToken = default(CancellationToken))
             where TTenant : class
        {
            optionsBuilder.ContainerEventsOptions.TenantContainerCreatedCallbacks.Add(async (a, sp) =>
            {
                var tenantShell = await a;
                var managerShellItem = await sp.GetShellItemAsync<TTenant, TenantHostedServiceManager<TTenant>>();
                await managerShellItem.StartAsync(cancellationToken).ConfigureAwait(false);
            });

            optionsBuilder.Builder.ConfigureTenantShellItem<TTenant, TenantHostedServiceManager<TTenant>>((a) =>
            {
                var manager = ActivatorUtilities.CreateInstance<TenantHostedServiceManager<TTenant>>(a.Services);
                configure(manager);
                return manager;
            });
            return optionsBuilder;
        }
    }
}
