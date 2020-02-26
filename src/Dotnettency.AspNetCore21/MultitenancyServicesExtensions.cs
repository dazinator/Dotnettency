using Dotnettency.Options;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AddAspNetCore<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
           where TTenant : class
        {
            builder.AddCoreAspNetCore<TTenant>();
            builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>));
            return builder;
        }
    }
}