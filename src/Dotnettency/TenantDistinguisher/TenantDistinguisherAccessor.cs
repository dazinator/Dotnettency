using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantDistinguisherAccessor<TTenant>
        where TTenant : class
    {
        private ITenantDistinguisherFactory<TTenant> _factory;

        public Lazy<Task<TenantDistinguisher>> TenantDistinguisher { get; private set; }

        public TenantDistinguisherAccessor(ITenantDistinguisherFactory<TTenant> factory)
        {
            _factory = factory;
            TenantDistinguisher = new Lazy<Task<TenantDistinguisher>>(() =>
            {
                return _factory.IdentifyContext();
            });
        }
    }
}
