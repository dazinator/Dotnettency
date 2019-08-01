using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public interface ITenantConfigurationAccessor<TTenant, TConfigBuilder, TConfiguration>
      where TTenant : class
    {
        Func<TConfigBuilder, IServiceProvider, ITenantConfigurationFactory<TTenant, TConfigBuilder, TConfiguration>, Lazy<Task<IConfiguration>>> BuildTenantConfigDelegate { get; }
    }

}
