using Dotnettency.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public class ModuleShellOptionsBuilder<TModule>
    {

        private ModuleShellOptions<TModule> _moduleShellOptions = new ModuleShellOptions<TModule>();

        public ModuleShellOptionsBuilder(TModule module)
        {
            Module = module;
            //   Tenant = tenant;
        }

        //public ModuleShellOptionsBuilder<TModule> UseIsolatedContainer()
        //{
        //    _moduleShellOptions.IsRoutedModule = true;
        //    return this;
        //}

        public TModule Module { get; set; }

        ////  public TTenant Tenant { get; set; }

        public ModuleShellOptionsBuilder<TModule> SetOnConfigureSharedServices(Action<IServiceCollection> onConfigure)
        {
            _moduleShellOptions.OnConfigureSharedServices = onConfigure;
            return this;
        }



        public ModuleShellOptionsBuilder<TModule> SetOnConfigureMiddleware(Action<IApplicationBuilder> onConfigureMiddleware)
        {
            _moduleShellOptions.OnConfigureMiddleware = onConfigureMiddleware;
            return this;
        }

        public ModuleShellOptionsBuilder<TModule> UseRouterFactory(Func<IApplicationBuilder, IRouter> router, Action<IServiceCollection> onConfigure)
        {
            _moduleShellOptions.OnConfigureModuleServices = onConfigure;
            _moduleShellOptions.GetRouter = router;
            return this;

        }
        internal ModuleShellOptions<TModule> Build()
        {
            return _moduleShellOptions;

        }

    }

}
