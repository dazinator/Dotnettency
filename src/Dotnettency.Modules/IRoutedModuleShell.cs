using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public interface IRoutedModuleShell<out TModule> : IModuleShell<TModule>
         where TModule : IModule
    {
        RequestDelegate MiddlewarePipeline { get; set; }
        IRouter Router { get; }
    }
}