using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.MiddlewarePipeline
{
    public class TenantPipelineOptionsBuilder<TTenant>
        where TTenant : class
    {
        private readonly MultitenancyOptionsBuilder<TTenant> _builder;

        public TenantPipelineOptionsBuilder(MultitenancyOptionsBuilder<TTenant> builder)
        {
            _builder = builder;
        }

        public MultitenancyOptionsBuilder<TTenant> OnInitialiseTenantPipeline(Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration)
        {
            var factory = new DelegateTenantMiddlewarePipelineFactory<TTenant>(configuration);
            _builder.Services.AddSingleton<ITenantMiddlewarePipelineFactory<TTenant>>(factory);
            _builder.Services.AddScoped<ITenantPipelineAccessor<TTenant>, TenantPipelineAccessor<TTenant>>();
            return _builder;
        }
    }
}
