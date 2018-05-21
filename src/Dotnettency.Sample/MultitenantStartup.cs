using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dotnettency;
using System;
using Dotnettency.Container;
using Dotnettency.AspNetCore;
using Dotnettency.AspNetCore.HostingEnvironment;
using Dotnettency.TenantFileSystem;

namespace Sample
{
    public abstract class MultitenantStartup<TTenant>
        where TTenant : class
    {
        private readonly IHostingEnvironment _environment;

        public MultitenantStartup(IHostingEnvironment environment, ILogger<MultitenantStartup<TTenant>> logger)
        {
            _environment = environment;
            Logger = logger;
        }

        public ILogger<MultitenantStartup<TTenant>> Logger { get; set; }


        protected IServiceProvider ConfigureMultitenancy(IServiceCollection services)
        {
            return ConfigureMultitenancy<RequestAuthorityTenantDistinguisherFactory<TTenant>>(services);
        }

        protected IServiceProvider ConfigureMultitenancy<TTenantDistinguisherFactory>(IServiceCollection services)
             where TTenantDistinguisherFactory : class, ITenantDistinguisherFactory<TTenant>
        {
            services.AddRouting();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var serviceProvider = services.AddMultiTenancy<TTenant>((options) =>
            {
                options.DistinguishTenantsWith<TTenantDistinguisherFactory>();
                ConfigureMultitenancy(options);
            });

            return serviceProvider;
        }


        protected abstract TenantShell<TTenant> ResolveTenantShell(TenantDistinguisher identifier);

        protected abstract AdaptedContainerBuilderOptions<TTenant> ConfigureContainerAndAdapt(ContainerBuilderOptions<TTenant> optionsBuilder, Action<TTenant, IServiceCollection> configureTenantServices);

        protected virtual void ConfigureMultitenancy(MultitenancyOptionsBuilder<TTenant> optionsBuilder)
        {
            optionsBuilder
                    .InitialiseTenant(ResolveTenantShell) // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
                    .ConfigureTenantContainers(ConfigureMultitenancyContainers)
                    .ConfigurePerTenantHostingEnvironment(_environment, ConfigureTenantHostingEnvironment);
        }

        protected virtual void ConfigureTenantContentRootFileSystem(TenantFileSystemBuilderContext<TTenant> options)
        {

        }

        protected virtual void ConfigureTenantWebRootFileSystem(TenantFileSystemBuilderContext<TTenant> options)
        {

        }   

        protected virtual void ConfigureTenantHostingEnvironment(TenantHostingEnvironmentOptionsBuilder<TTenant> optionsBuilder)
        {
            optionsBuilder.OnInitialiseTenantContentRoot(ConfigureTenantContentRootFileSystem);
            optionsBuilder.OnInitialiseTenantWebRoot(ConfigureTenantWebRootFileSystem);
        }

        protected virtual void ConfigureTenantContainerEvents(TenantContainerEventsOptions<TTenant> optionsBuilder)
        {

        }

        protected virtual void ConfigureTenantServices(TTenant currentTenant, IServiceCollection tenantServices)
        {

        }

        protected virtual void ConfigureMultitenancyContainers(ContainerBuilderOptions<TTenant> optionsBuilder)
        {
            var options = optionsBuilder.Events(ConfigureTenantContainerEvents);
            var adapted = ConfigureContainerAndAdapt(options, ConfigureTenantServices);
            adapted.AddPerRequestContainerMiddlewareServices();
            adapted.AddPerTenantMiddlewarePipelineServices();
        }

    }
}
