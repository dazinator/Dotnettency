using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public class MultitenancyMiddlewareOptionsBuilder<TTenant>
    {
        public IApplicationBuilder ApplicationBuilder { get; set; }

        public MultitenancyMiddlewareOptionsBuilder(IApplicationBuilder app)
        {
            ApplicationBuilder = app;
        }
    }
}