using Dotnettency.Configuration;
using Dotnettency.MiddlewarePipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantPipelineBuilderContextConfigurationExtensions
    {
        public static async Task<IConfiguration> GetConfiguration<TTenant>(this TenantPipelineBuilderContext<TTenant> context, IServiceProvider serviceProvider)
            where TTenant : class
        {
            var accessor = serviceProvider.GetRequiredService<ITenantConfigurationAccessor<TTenant, IConfiguration>>();
            var accessDelegate = accessor.ConfigFactory(serviceProvider);
            var config = await accessDelegate.Value;
            return config;
        }
    }

}
