using System.Threading.Tasks;
using Dotnettency;
using Microsoft.AspNetCore.Routing;

namespace Sample
{
    public class TenantContainerRouter<TTenant> : INamedRouter
        where TTenant : class
    {
        private readonly ITenantDistinguisherFactory<TTenant> _tenantDistinguisherFactory;

        public TenantContainerRouter(string name, ITenantDistinguisherFactory<TTenant> tenantDistinguisherFactory)
        {
            Name = name;
            _tenantDistinguisherFactory = tenantDistinguisherFactory;
        }

        public string Name { get; set; }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            throw new System.NotImplementedException();
           // return new VirtualPathData(this,"")
        }

        public Task RouteAsync(RouteContext context)
        {

          //  context.han

            var tenantDistinguisher = _tenantDistinguisherFactory.IdentifyContext();
            throw new System.NotImplementedException();
        }
    }
}
