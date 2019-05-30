using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Sample.AspNetCore30.RazorPages.Internal
{
    /// <summary>
    /// Allows fine grained configuration of MVC services.
    /// </summary>
    internal class MvcBuilder : IMvcBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="MvcBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="manager">The <see cref="ApplicationPartManager"/> of the application.</param>
        public MvcBuilder(IServiceCollection services, ApplicationPartManager manager)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            Services = services;
            PartManager = manager;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public ApplicationPartManager PartManager { get; }
    }
}
