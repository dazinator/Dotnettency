using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public class MultitenancyMiddlewareOptionsBuilder<TTenant>
    {
        public MultitenancyMiddlewareOptionsBuilder(IApplicationBuilder app)
        {
            ApplicationBuilder = app;
        }

        public IApplicationBuilder ApplicationBuilder { get; set; }
    }
}