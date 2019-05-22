using Dotnettency.TenantFileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.AspNetCore.HostingEnvironment
{
    public class TenantHostingEnvironmentOptionsBuilder<TTenant>
        where TTenant : class
    {
        private readonly IHostingEnvironment _parentHostingEnvironment;

        /// <summary>
        /// Add this to Tenant Container.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="hostingEnvironment"></param>
        public TenantHostingEnvironmentOptionsBuilder(
            MultitenancyOptionsBuilder<TTenant> builder, 
            IHostingEnvironment hostingEnvironment)
        {
            Builder = builder;
            _parentHostingEnvironment = hostingEnvironment;

            // Wrap the singleton registration of IHostingEnvironment with
            // one that resolves current tenant specific file providers.
            Builder.Services.AddSingleton<IHostingEnvironment>((sp) =>
            {
                var contextProvider = sp.GetRequiredService<IHttpContextProvider>();
                var factory = sp.GetRequiredService<IHttpContextProvider>();
                var webRootfactory = sp.GetRequiredService<ITenantWebRootFileSystemProviderFactory<TTenant>>();
                var contentRootfactory = sp.GetRequiredService<ITenantContentRootFileSystemProviderFactory<TTenant>>();
                return new TenantHostingEnvironment<TTenant>(hostingEnvironment, contextProvider, webRootfactory, contentRootfactory);
            });
        }

        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }

        public MultitenancyOptionsBuilder<TTenant> OnInitialiseTenantContentRoot(Action<TenantFileSystemBuilderContext<TTenant>> configureContentRoot)
        {
            var factory = new DelegateTenantContentRootFileSystemProviderFactory<TTenant>(_parentHostingEnvironment, configureContentRoot);
            Builder.Services.AddSingleton<ITenantContentRootFileSystemProviderFactory<TTenant>>(factory);
            return Builder;
        }

        public MultitenancyOptionsBuilder<TTenant> OnInitialiseTenantWebRoot(Action<TenantFileSystemBuilderContext<TTenant>> configureWebRoot)
        {
            var factory = new DelegateTenantWebRootFileSystemProviderFactory<TTenant>(_parentHostingEnvironment, configureWebRoot);
            Builder.Services.AddSingleton<ITenantWebRootFileSystemProviderFactory<TTenant>>(factory);
            return Builder;
        }
    }
}
