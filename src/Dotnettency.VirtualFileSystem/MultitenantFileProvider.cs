using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.Extensions.Primitives;
using DotNet.Cabinets;
using System.Threading.Tasks;

namespace Dotnettency.TenantFileSystem
{
    public class MultitenantFileProvider<TTenant> : IFileProvider
        where TTenant : class
    {
        private readonly IHttpContextProvider _contextprovider;
        private readonly string _name;


        public IFileProvider GetActiveFileProvider()
        {
            var currentContext = _contextprovider.GetCurrent();
            if (currentContext == null)
            {
                return null;
            }
            var cabinet = GetCabinet(currentContext).Result;
            var fileProvider = cabinet?.FileProvider;
            return fileProvider;
        }

        public MultitenantFileProvider(
            IHttpContextProvider contextprovider,
            // ITenantFileSystemProviderFactory<TTenant> rootFactory,
            string name = "")
        {
            _contextprovider = contextprovider;
            _name = name;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var fp = GetActiveFileProvider();
            if (fp == null)
            {
                return new NotFoundDirectoryContents();
            }
            return fp.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fp = GetActiveFileProvider();
            if (fp == null)
            {
                return new NotFoundFileInfo(subpath);
            }
            return fp.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            var fp = GetActiveFileProvider();
            if (fp == null)
            {
                return NullChangeToken.Singleton;
            }
            return fp.Watch(filter);
        }

        protected async Task<TenantShell<TTenant>> GetTenantShell(HttpContextBase context)
        {
            var tenantShell = await context?.GetTenantShell<TTenant>();
            return tenantShell;
        }

        protected async Task<ICabinet> GetCabinet(HttpContextBase context)
        {
            var services = context.GetRequestServices();
            if(services == null)
            {
                return null;
            }

            var cabinet = await services.GetShellItemAsync<TTenant, ICabinet>(_name);
            return cabinet;
        }
    }
}
