using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.SystemWeb
{
    public static class IServiceCollectionExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AddSystemWeb<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            builder.Services.AddSingleton<IHttpContextProvider, HttpContextProvider>();
            return builder;           
        }
    }
}
