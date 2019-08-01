using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public interface ITenantConfigurationFactory<TTenant, TConfigurationBuilder, TConfiguration>
    where TTenant : class
    {
        Task<TConfiguration> Create(TConfigurationBuilder builder, IServiceProvider serviceProviderOverride, TTenant tenant);
    }

}
