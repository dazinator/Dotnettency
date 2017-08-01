using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public interface IModuleManager<TModule>
        where TModule : IModule
    {
        Task EnsureModulesStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder);

        ModulesRouter<TModule> GetModulesRouter();
    }
}
