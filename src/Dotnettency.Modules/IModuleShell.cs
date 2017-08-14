using System;
using System.Threading.Tasks;
using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{

    public interface IModuleShell<TModule>
    {
        IApplicationBuilder AppBuilder { get; }
        ITenantContainerAdaptor Container { get; set; }
        bool IsStarted { get; }
        TModule Module { get; }
        IRouter Router { get; }
        ModuleShellOptions<TModule> Options { get; }
        Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices);
    }
}