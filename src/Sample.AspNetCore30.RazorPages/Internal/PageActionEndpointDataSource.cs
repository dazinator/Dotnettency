using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sample.Pages
{

    internal class PageActionEndpointDataSource : ActionEndpointDataSourceBase
    {
        private readonly ActionEndpointFactory _endpointFactory;

        public PageActionEndpointDataSource(IActionDescriptorCollectionProvider actions, ActionEndpointFactory endpointFactory)
            : base(actions)
        {
            _endpointFactory = endpointFactory;


            DefaultBuilder = new PageActionEndpointConventionBuilder(Lock, Conventions);

            // IMPORTANT: this needs to be the last thing we do in the constructor. 
            // Change notifications can happen immediately!
            Subscribe();
        }

        public PageActionEndpointConventionBuilder DefaultBuilder { get; }

        protected override List<Endpoint> CreateEndpoints(IReadOnlyList<ActionDescriptor> actions, IReadOnlyList<Action<EndpointBuilder>> conventions)
        {
            var endpoints = new List<Endpoint>();
            var routeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < actions.Count; i++)
            {
                if (actions[i] is PageActionDescriptor action)
                {
                    _endpointFactory.AddEndpoints(endpoints, routeNames, action, Array.Empty<ConventionalRouteEntry>(), conventions);
                }
            }

            return endpoints;
        }
    }
}
