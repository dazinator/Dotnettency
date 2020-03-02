using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Dotnettency
{
    public static class LoggingServiceCollectionExtensions
    {

        /// <summary>
        /// Configure the <see cref="ILogProvider"/>s explicitly by adding a logging factory instance. 
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddLoggingFactory(this IServiceCollection services, Action<ILoggingBuilder> configure)
        {
            var loggerFactory = LoggerFactory.Create(configure);
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            return services;
        }
    }
}