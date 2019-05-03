using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantIdentifierAccessor<TTenant>
        where TTenant : class
    {
        private ITenantIdentifierFactory<TTenant> _factory;

        public TenantIdentifierAccessor(ITenantIdentifierFactory<TTenant> factory)
        {
            _factory = factory;
            TenantDistinguisher = new Lazy<Task<TenantIdentifier>>(() =>
            {
                return _factory.IdentifyTenant();
            });
        }

        public Lazy<Task<TenantIdentifier>> TenantDistinguisher { get; private set; }
    }
}
