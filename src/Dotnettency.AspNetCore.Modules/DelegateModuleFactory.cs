using Dotnettency.Container;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public class DelegateModuleFactory<TTenant, TModule> : IModuleFactory<TTenant, TModule>
         where TTenant : class
         where TModule : IModule
    {

        private readonly Func<ITenantContainerAdaptor, TTenant, IEnumerable<TModule>> _getModulesDelegate;

        public DelegateModuleFactory(Func<ITenantContainerAdaptor, TTenant, IEnumerable<TModule>> getModulesDelegate)
        {
            _getModulesDelegate = getModulesDelegate;
        }

        public Task<IEnumerable<TModule>> GetModulesForTenant(ITenantContainerAdaptor container, TTenant tenant)
        {
            return Task.Run(() =>
            {
                return _getModulesDelegate(container, tenant);
            });
        }
    }

    //public class ModuleShell<TModule>
    //    where TModule : IModule
    //{

    //    private readonly Func<ITenantContainerAdaptor> _moduleContainerFactory;

    //    public ModuleShell(IModule module, Func<ITenantContainerAdaptor> moduleContainerFactory)
    //    {
    //        Module = module;
    //        _moduleContainerFactory = moduleContainerFactory;
    //    }

    //    public IModule Module { get; }

    //    public void Restart()
    //    {
    //        if (Container != null)
    //        {
    //            Container.Dispose();
    //            Container = null;
    //        }
    //        // Get the module container
    //        Container = _moduleContainerFactory();



    //    }

    //    /// <summary>
    //    /// The curent container that is used for this modules services.
    //    /// </summary>
    //    /// <remarks>Could be the Application, Tenant, or Module container depending on how modules are being isolated.</remarks>
    //    private ITenantContainerAdaptor Container { get; set; }

    //}
}
