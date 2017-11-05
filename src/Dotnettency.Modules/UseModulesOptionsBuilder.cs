using Dotnettency.Modules;
using Microsoft.AspNetCore.Builder;

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
            _parent.ApplicationBuilder.UseMiddleware<ModulesMiddleware<TTenant, TModule>>(_parent.ApplicationBuilder);
            return _parent;
        }
    }
}
