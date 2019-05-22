using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.Extensions.DependencyInjection;
using Dotnettency.AspNetCore.HostingEnvironment;
using DotNet.Cabinets;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.HostingEnvironment
{
    /// <summary>
    /// Should be registered as Singleton to override default IHostingEnvironment implementation.
    /// This one is sensitive to current Tenant resolved during request, and if found, will return that tenants IFileProviders instead of the root level.
    /// </summary>
    /// <typeparam name="TTenant"></typeparam>
    public class TenantHostingEnvironment<TTenant> : IHostingEnvironment
        where TTenant : class
    {

        public Guid InstanceId { get; set; } = Guid.NewGuid();
        public TenantHostingEnvironment(
            IHostingEnvironment parent,
            IHttpContextProvider contextprovider,
            ITenantWebRootFileSystemProviderFactory<TTenant> webRootfactory,
            ITenantContentRootFileSystemProviderFactory<TTenant> contentRootFactory)
        {
            // Default to parent hosting environment, but allow properties to be overriden for the tenant.          
            Parent = parent;
            Contextprovider = contextprovider;
            WebRootFactory = webRootfactory;
            ContentRootFactory = contentRootFactory;                 


            EnvironmentName = parent.EnvironmentName;
            ApplicationName = parent.ApplicationName;
            WebRootPath = parent.WebRootPath;
            WebRootFileProvider = parent.WebRootFileProvider;
            ContentRootPath = parent.ContentRootPath;
            ContentRootFileProvider = parent.ContentRootFileProvider;
        }

        public IHostingEnvironment Parent { get; set; }
        public IHttpContextProvider Contextprovider { get; }
        public ITenantWebRootFileSystemProviderFactory<TTenant> WebRootFactory { get; }
        public ITenantContentRootFileSystemProviderFactory<TTenant> ContentRootFactory { get; }
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public async Task<TenantShell<TTenant>> GetTenantShell()
        {
            var currentContext = Contextprovider.GetCurrent();
            var tenantShell = await currentContext?.GetTenantShell<TTenant>();
            return tenantShell;
        }

        public async Task<ICabinet> GetContentCabinet()
        {
            var tenantShell = await GetTenantShell();
            var lazyFactory = new Lazy<ICabinet>(() =>
            {
                return ContentRootFactory.GetContentRoot(tenantShell.Tenant);
            });
            var cabinet = tenantShell?.GetOrAddTenantContentRootFileSystem(lazyFactory)?.Value;
            return cabinet;
        }

        public async Task<ICabinet> GetWebRootCabinet()
        {
            var tenantShell = await GetTenantShell();
            var lazyFactory = new Lazy<ICabinet>(() =>
            {
                return WebRootFactory.GetWebRoot(tenantShell.Tenant);
            });
            var cabinet = tenantShell?.GetOrAddTenantWebRootFileSystem(lazyFactory)?.Value;
            return cabinet;
        }

        public IFileProvider WebRootFileProvider
        {
            get
            {
                var webRoot = GetWebRootCabinet().Result?.FileProvider ?? Parent.WebRootFileProvider;
                return webRoot;
            }
            set
            {
                Parent.WebRootFileProvider = value;
            }
        }

        public IFileProvider ContentRootFileProvider
        {
            get
            {
                var webRoot = GetContentCabinet().Result?.FileProvider ?? Parent.ContentRootFileProvider;
                return webRoot;
            }
            set
            {
                Parent.ContentRootFileProvider = value;
            }
        }

        public string ContentRootPath { get; set; }

    }
}
