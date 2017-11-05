using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Container
{
    public class ContainerBuilderOptions<TTenant>
        where TTenant : class
    {
        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }

        public ContainerBuilderOptions(MultitenancyOptionsBuilder<TTenant> builder)
        {
            Builder = builder;
            builder.Services.AddSingleton<ITenantContainerFactory<TTenant>, TenantContainerBuilderFactory<TTenant>>();
            builder.Services.AddScoped<ITenantContainerAccessor<TTenant>, TenantContainerAccessor<TTenant>>();
            builder.Services.AddScoped<ITenantRequestContainerAccessor<TTenant>, TenantRequestContainerAccessor<TTenant>>();            
        }
    }
}
