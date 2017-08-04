using Dotnettency.Modules;
using System;

namespace Dotnettency
{
    public class DelegateModuleShellFactory<TTenant, TModule> : IModuleShellFactory<TTenant, TModule>
        where TTenant : class
         where TModule : IModule
    {

        private readonly Func<TTenant, TModule, ModuleShell<TModule>> _factoryDelegate;

        public DelegateModuleShellFactory(Func<TTenant, TModule, ModuleShell<TModule>> factoryDelegate)
        {
            _factoryDelegate = factoryDelegate;
        }

        public ModuleShell<TModule> GetModuleShell(TTenant tenant, TModule module)
        {
            return _factoryDelegate(tenant, module);
        }
    }

}