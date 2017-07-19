using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantAccessor<TTenant> where TTenant : class
    {
        Lazy<Task<TTenant>> CurrentTenant { get; }      
    }
}