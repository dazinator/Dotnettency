using System;
using System.Threading.Tasks;
using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    public interface IModuleShell<out TModule>
        where TModule : IModule
    {
        IApplicationBuilder AppBuilder { get; }
        ITenantContainerAdaptor Container { get; set; }
        bool IsStarted { get; }
        RequestDelegate MiddlewarePipeline { get; set; }
        IModule Module { get; set; }
        ModuleShellOptions Options { get; }
        IRouter Router { get; }

        Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices);
    }
}