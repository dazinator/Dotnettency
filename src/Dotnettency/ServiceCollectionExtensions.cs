using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnettency
{
    public static class ServiceCollectionExtensions
    {
        public static T FindServiceInstance<T>(this IServiceCollection services)
        {
            var registered = services.LastOrDefault((s) =>
            {
                return s.ServiceType == typeof(T);
            });

            T instance = default(T);
            if (registered == null)
            {
                return instance;
            }
            else
            {
                if (registered.ImplementationInstance != null)
                {
                    instance = (T)registered.ImplementationInstance;
                }
                else if (registered.ImplementationFactory != null)
                {
                    throw new System.NotImplementedException();
                }
                else
                {
                    instance = (T)Activator.CreateInstance(registered.ImplementationType);
                }
                return instance;
            }
        }
    }
}
