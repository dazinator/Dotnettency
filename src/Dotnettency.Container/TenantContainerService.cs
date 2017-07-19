using System;
using System.Threading.Tasks;

namespace WebExperiment
{
    public class TenantContainerService<TTenant> : TenantKeyedService<TTenant, ITenantContainerAdaptor>
       where TTenant : class
    {

        private readonly ITenantContainerFactory<TTenant> _resolver;

        //private readonly ConcurrentDictionary<string, IServiceProvider> _containers;

        public TenantContainerService(ITenantContainerFactory<TTenant> resolver)
        {
            _resolver = resolver;
            //_containers = new ConcurrentDictionary<string, IServiceProvider>();
        }

        public override async Task<ITenantContainerAdaptor> Get(TenantIdentifier identifier)
        {
            // return new Task<TTenant>(async () =>
            //{
            if (identifier == null)
            {
                return null;
            }

            var container = await _resolver.Get(identifier);
            return container;
            // });


        }
    }





}
