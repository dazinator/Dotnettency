using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellAccessor<TTenant> where TTenant : class
    {
        Lazy<Task<TenantShell<TTenant>>> CurrentTenantShell { get; }
    }
}