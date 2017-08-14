using Dotnettency.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public class UseModulesOptionsBuilder<TTenant>
        where TTenant : class
    {
        private MultitenancyMiddlewareOptionsBuilder<TTenant> _parent;

        public UseModulesOptionsBuilder(MultitenancyMiddlewareOptionsBuilder<TTenant> parent)
        {
            _parent = parent;
        }

        public MultitenancyMiddlewareOptionsBuilder<TTenant> OfType<TModule>()
        {
           // var container = _parent.ApplicationBuilder.ApplicationServices;
           // var resolved = container.GetRequiredService(typeof(IModuleManager<ModuleBase>));
            _parent.ApplicationBuilder.UseMiddleware<ModulesMiddleware<TTenant, TModule>>(_parent.ApplicationBuilder);

            //  _parent.ApplicationBuilder.UseMiddleware<ModulesMiddleware<TTenant, TModule>>(_parent.ApplicationBuilder);
            return _parent;
        }
    }

}