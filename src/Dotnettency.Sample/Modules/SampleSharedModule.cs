using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dotnettency.Modules;

namespace Sample
{
    public class SampleSharedModule : SharedModuleBase
    {
        public override void ConfigureMiddleware(IApplicationBuilder appBuilder)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {

        }
    }
}
