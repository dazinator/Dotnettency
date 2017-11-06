using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.HostingEnvironment
{
    public class TenantHostingEnvironmentOptionsBuilder<TTenant>
        where TTenant : class
    {
        private readonly IHostingEnvironment _parentHostingEnvironment;

        public TenantHostingEnvironmentOptionsBuilder(MultitenancyOptionsBuilder<TTenant> builder, IHostingEnvironment hostingEnvironment)
        {
            Builder = builder;
            _parentHostingEnvironment = hostingEnvironment;

            // Override the singleton registration of IHostingEnvironment with
            // a scoped version so that we can configure it for the tenant
            // early on in each request.
            Builder.Services.AddScoped<IHostingEnvironment>((sp) =>
            {
                return new TenantHostingEnvironment<TTenant>(hostingEnvironment);
            });
        }

        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }

        public MultitenancyOptionsBuilder<TTenant> OnInitialiseTenantContentRoot(Action<TenantFileSystemBuilderContext<TTenant>> configureContentRoot)
        {
            var factory = new DelegateTenantContentRootFileSystemProviderFactory<TTenant>(_parentHostingEnvironment, configureContentRoot);
            Builder.Services.AddSingleton<ITenantContentRootFileSystemProviderFatory<TTenant>>(factory);
            return Builder;
        }

        public MultitenancyOptionsBuilder<TTenant> OnInitialiseTenantWebRoot(Action<TenantFileSystemBuilderContext<TTenant>> configureWebRoot)
        {
            var factory = new DelegateTenantWebRootFileSystemProviderFactory<TTenant>(_parentHostingEnvironment, configureWebRoot);
            Builder.Services.AddSingleton<ITenantWebRootFileSystemProviderFatory<TTenant>>(factory);
            return Builder;
        }
    }
}
