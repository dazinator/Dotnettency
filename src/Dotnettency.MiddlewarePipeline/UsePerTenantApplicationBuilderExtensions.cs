using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{

    public static class UsePerTenantApplicationBuilderExtensions
    {

        public static IApplicationBuilder UsePerTenantMiddlewarePipeline<TTenant>(this IApplicationBuilder app)
            where TTenant : class
        {

            // Ensure.Argument.NotNull(app, nameof(app));

            // Ensure.Argument.NotNull(configuration, nameof(configuration));


            app.UseMiddleware<TenantPipelineMiddleware<TTenant>>(app);
            //   app.Use(next => new TenantPipelineMiddleware<TTenant>(next, app, configuration).Invoke);

            return app;

        }

    }

}

