using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public interface IModuleManager<TModule>
    {
        Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder);
        IRouter GetModulesRouter();
    }
}
