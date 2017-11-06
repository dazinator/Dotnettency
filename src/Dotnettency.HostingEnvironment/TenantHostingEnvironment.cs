using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Dotnettency.HostingEnvironment
{
    public class TenantHostingEnvironment<TTenant> : IHostingEnvironment
        where TTenant : class
    {
        public TenantHostingEnvironment(IHostingEnvironment parent)
        {
            // Default to parent hosting environment, but allow properties to be overriden for the tenant.
            Parent = parent;

            EnvironmentName = parent.EnvironmentName;
            ApplicationName = parent.ApplicationName;
            WebRootPath = parent.WebRootPath;
            WebRootFileProvider = parent.WebRootFileProvider;
            ContentRootPath = parent.ContentRootPath;
            ContentRootFileProvider = parent.ContentRootFileProvider;
        }

        public IHostingEnvironment Parent { get; set; }
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
