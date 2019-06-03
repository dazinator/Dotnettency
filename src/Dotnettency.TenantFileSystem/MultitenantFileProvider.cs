using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.Extensions.Primitives;
using DotNet.Cabinets;
using System.Threading.Tasks;

namespace Dotnettency.TenantFileSystem
{
    public class CurrentTenantFileProvider<TTenant> : IFileProvider
        where TTenant : class
    {
        private readonly IHttpContextProvider _contextprovider;
        private readonly ITenantFileSystemProviderFactory<TTenant> _rootFactory;
        private readonly string _key;


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

        public CurrentTenantFileProvider(
            IHttpContextProvider contextprovider,
            ITenantFileSystemProviderFactory<TTenant> rootFactory,
            string key)
        {
            _contextprovider = contextprovider;
            _rootFactory = rootFactory;
            _key = key;
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
            var tenantShell = await GetTenantShell(context);
            if(tenantShell == null)
            {
                return null;
            }
            //todo: store this factory in tenant shell so it can be reused until tenant is reset..
            var lazyFactory = new Lazy<ICabinet>(() =>
            {
                return _rootFactory.GetRoot(tenantShell.Tenant);
            });
            var cabinet = tenantShell?.GetOrAddTenantFileSystem(_key, lazyFactory)?.Value;
            return cabinet;
        }
    }
}
