using System;
using System.Threading.Tasks;
using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{

    public interface IModuleShell<out TModule>
        where TModule : IModule
    {
        IApplicationBuilder AppBuilder { get; }
        ITenantContainerAdaptor Container { get; set; }
        bool IsStarted { get; }
        IModule Module { get; set; }
        ModuleShellOptions Options { get; }
        Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices);
    }
}