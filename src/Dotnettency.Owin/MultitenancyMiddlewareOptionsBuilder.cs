using Owin;

namespace Dotnettency.Owin
{
    public class MultitenancyMiddlewareOptionsBuilder<TTenant>
    {
        public MultitenancyMiddlewareOptionsBuilder(IAppBuilder app)
        {
            ApplicationBuilder = app;
        }

        public IAppBuilder ApplicationBuilder { get; set; }
    }
}