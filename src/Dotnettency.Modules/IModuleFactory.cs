using Dotnettency.Container;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public interface IModuleFactory<TTenant, TModule>
        where TModule : IModule
    {
        /// <summary>
        /// Returns all the modules that are enabled for a particular tenant.
        /// </summary>
        /// <param name="tenant">The tenant for whom modules need to be retreived.</param>
        /// <param name="container">Container that can be used to activate modules for this tenant. Typically the tenants container.</param>
        /// <returns></returns>
        Task<IEnumerable<TModule>> GetModulesForTenant(ITenantContainerAdaptor container, TTenant tenant);
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
