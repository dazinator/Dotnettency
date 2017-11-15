using Dotnettency.Modules;

namespace Dotnettency
{
    public interface IModuleShellFactory<TTenant, TModule>
         where TTenant : class
         where TModule : IModule
    {
        ModuleShell<TModule> GetModuleShell(TTenant tenant, TModule module);
    }

}