using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Sample
{
    public class MultiTenancyRouteBuilder : IRouteBuilder
    {
        public MultiTenancyRouteBuilder(IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            ApplicationBuilder = builder;
            ServiceProvider = serviceProvider;
            DefaultHandler = null;
            Routes = new List<IRouter>();
        }

        public IApplicationBuilder ApplicationBuilder { get; set; }

        public IRouter DefaultHandler { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public IList<IRouter> Routes { get; set; }

        public IRouter Build()
        {
            //return new ContainerRouter()
             throw new NotImplementedException();
        }
    }
}
