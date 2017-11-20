using Dotnettency.AspNetCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
