using Dotnettency.HostingEnvironment;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public class PerTenantHostingEnvironmentMiddlewareOptionsBuilder<TTenant>
        where TTenant : class
    {
        private MultitenancyMiddlewareOptionsBuilder<TTenant> _builder;

        public PerTenantHostingEnvironmentMiddlewareOptionsBuilder(MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
        {
            _builder = builder;         
        }

        public PerTenantHostingEnvironmentMiddlewareOptionsBuilder<TTenant> UseTenantContentRootFileProvider()
        {
            _builder.ApplicationBuilder.UseMiddleware<TenantHostingEnvironmentContentRootMiddleware<TTenant>>(_builder.ApplicationBuilder);
            return this;
        }

        public PerTenantHostingEnvironmentMiddlewareOptionsBuilder<TTenant> UseTenantWebRootFileProvider()
        {
            _builder.ApplicationBuilder.UseMiddleware<TenantHostingEnvironmentWebRootMiddleware<TTenant>>(_builder.ApplicationBuilder);
            return this;
        }
    }
}
