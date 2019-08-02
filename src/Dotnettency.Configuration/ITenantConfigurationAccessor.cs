using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public interface ITenantConfigurationAccessor<TTenant, TConfiguration>
      where TTenant : class
    {
        Func<IServiceProvider, Lazy<Task<IConfiguration>>> ConfigFactory { get; }
    }

}
