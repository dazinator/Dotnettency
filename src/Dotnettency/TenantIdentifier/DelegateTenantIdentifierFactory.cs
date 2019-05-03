using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class DelegateTenantIdentifierFactory<TTenant> : ITenantIdentifierFactory<TTenant>
       where TTenant : class
    {
        private readonly Func<Task<TenantIdentifier>> _getDistinguisher;

        public DelegateTenantIdentifierFactory(Func<Task<TenantIdentifier>> getDistinguisher) : base()
        {
            _getDistinguisher = getDistinguisher;
        }

        public Task<TenantIdentifier> IdentifyTenant()
        {
            return _getDistinguisher?.Invoke();
        }      
    }
}
