using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public static IServiceCollection Clone(this IServiceCollection services)
        {
            var newServices = new ServiceCollection();
            var collection = (ICollection<ServiceDescriptor>)newServices;
            foreach (ServiceDescriptor descriptor in services)
            {
                collection.Add(descriptor);
            }
            return newServices;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public static IServiceCollection AddRange(this IServiceCollection services, IEnumerable<ServiceDescriptor> range)
        {
            foreach (ServiceDescriptor descriptor in range)
            {
                services.Add(descriptor);
            }
            return services;
        }

    }
}
