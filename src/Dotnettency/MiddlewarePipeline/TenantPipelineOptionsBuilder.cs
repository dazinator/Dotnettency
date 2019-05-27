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

        public MultitenancyOptionsBuilder<TTenant> MultitenancyOptions { get { return _builder; } }
    }
}
