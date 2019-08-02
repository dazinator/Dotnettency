using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public interface ITenantConfigurationFactory<TTenant, TConfiguration>
    where TTenant : class
    {
        Task<TConfiguration> Create(IServiceProvider serviceProviderOverride, TTenant tenant);
    }

}
