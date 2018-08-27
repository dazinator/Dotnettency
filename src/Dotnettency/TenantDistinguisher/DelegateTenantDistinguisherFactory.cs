using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class DelegateTenantDistinguisherFactory<TTenant> : ITenantDistinguisherFactory<TTenant>
       where TTenant : class
    {
        private readonly Func<Task<TenantDistinguisher>> _getDistinguisher;

        public DelegateTenantDistinguisherFactory(Func<Task<TenantDistinguisher>> getDistinguisher) : base()
        {
            _getDistinguisher = getDistinguisher;
        }

        public Task<TenantDistinguisher> IdentifyContext()
        {
            return _getDistinguisher?.Invoke();
        }      
    }
}
