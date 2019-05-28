using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace Sample.Pages
{
    /// <summary>
    /// Builds conventions that will be used for customization of <see cref="EndpointBuilder"/> instances.
    /// </summary>
    /// <remarks>
    /// This interface is used at application startup to customize endpoints for the application.
    /// </remarks>
    public sealed class PageActionEndpointConventionBuilder : IEndpointConventionBuilder
    {
        // The lock is shared with the data source.
        private readonly object _lock;
        private readonly List<Action<EndpointBuilder>> _conventions;

        internal PageActionEndpointConventionBuilder(object @lock, List<Action<EndpointBuilder>> conventions)
        {
            _lock = @lock;
            _conventions = conventions;
        }

        /// <summary>
        /// Adds the specified convention to the builder. Conventions are used to customize <see cref="EndpointBuilder"/> instances.
        /// </summary>
        /// <param name="convention">The convention to add to the builder.</param>
        public void Add(Action<EndpointBuilder> convention)
        {
            if (convention == null)
            {
                throw new ArgumentNullException(nameof(convention));
            }

            // The lock is shared with the data source. We want to lock here
            // to avoid mutating this list while its read in the data source.
            lock (_lock)
            {
                _conventions.Add(convention);
            }
        }
    }
}
