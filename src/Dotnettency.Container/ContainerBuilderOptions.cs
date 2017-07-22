using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Container
{

    public class ContainerBuilderOptions<TTenant>
        where TTenant : class
    {

        public ContainerBuilderOptions(MultitenancyOptionsBuilder<TTenant> builder)
        {
            Builder = builder;
            builder.Services.AddSingleton<ITenantContainerFactory<TTenant>, TenantContainerBuilderFactory<TTenant>>();
        }

        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }


        // public IServiceCollection Services { get; set; }

    }


}