using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public static class TenantShellConfigurationExtensions
    {
        public static Lazy<Task<IConfiguration>> GetOrAddConfiguration<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<Task<IConfiguration>> requestDelegateFactory)
            where TTenant : class
        {
            return tenantShell.GetOrAddProperty<Lazy<Task<IConfiguration>>>(nameof(TenantShellConfigurationExtensions), requestDelegateFactory);
        }
    }
}
