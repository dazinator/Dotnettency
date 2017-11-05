using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public class ModuleShellOptionsBuilder<TModule>
    {
        private ModuleShellOptions<TModule> _moduleShellOptions = new ModuleShellOptions<TModule>();

        public TModule Module { get; set; }

        public ModuleShellOptionsBuilder(TModule module)
        {
            Module = module;
        }

        public ModuleShellOptionsBuilder<TModule> HasSharedServices(Action<IServiceCollection> onConfigure)
        {
            _moduleShellOptions.OnConfigureSharedServices = onConfigure;
            return this;
        }

        public ModuleShellOptionsBuilder<TModule> HasMiddlewareConfiguration(Action<IApplicationBuilder> onConfigureMiddleware)
        {
            _moduleShellOptions.OnConfigureMiddleware = onConfigureMiddleware;
            return this;
        }

        public ModuleShellOptionsBuilder<TModule> HasRoutedContainer(Func<IApplicationBuilder, IRouter> router, Action<IServiceCollection> onConfigure)
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
