using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;

namespace Dotnettency.HostingEnvironment
{
    public class TenantHostingEnvironment<TTenant> : IHostingEnvironment
        where TTenant : class
    {

        // private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantHostingEnvironment(IHostingEnvironment parent)
        {
            // Default to parent hosting environment, but allow properties to be overriden for the tenant.
            Parent = parent;
            //   _tenantShellAccessor = tenantShellAccessor;

            EnvironmentName = parent.EnvironmentName;
            ApplicationName = parent.ApplicationName;
            WebRootPath = parent.WebRootPath;
            WebRootFileProvider = parent.WebRootFileProvider;
            ContentRootPath = parent.ContentRootPath;
            ContentRootFileProvider = parent.ContentRootFileProvider;
        }

        public IHostingEnvironment Parent { get; set; }

        public string EnvironmentName
        {
            get; set;
        }

        //private string _ApplicationName;
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
