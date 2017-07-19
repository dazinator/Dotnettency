using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    // scoped so that each request has 
    public class TenantDistinguisherAccessor<TTenant>
        where TTenant : class
    {
        private ITenantDistinguisherFactory<TTenant> _factory;

        public TenantDistinguisherAccessor(ITenantDistinguisherFactory<TTenant> factory)
        {
            _factory = factory;
            TenantDistinguisher = new Lazy<Task<TenantDistinguisher>>(() =>
            {
                return _factory.IdentifyContext();
            });
        }


        public Lazy<Task<TenantDistinguisher>> TenantDistinguisher { get; }
    }





}
