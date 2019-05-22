using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{

    public abstract class HttpContextBase
    {
        protected const int DefaultHttpPort = 80;
        protected const int DefaultHttpsPort = 443;

        public abstract RequestBase Request { get; }

        public abstract void SetItem(string key, object item);

        public abstract void SetItem(string key, IDisposable item, bool disposeOnRequestCompletion = true);
        public abstract TItem GetItem<TItem>(string key)
            where TItem : class;

        public abstract bool ContainsKey(string key);

        public abstract void SetRequestServices(IServiceProvider requestServices);

        public abstract IServiceProvider GetRequestServices();

        public Task<TenantShell<TTenant>> GetTenantShell<TTenant>()
            where TTenant : class
        {
            var tenantShellAccessor = GetRequestServices().GetService<ITenantShellAccessor<TTenant>>();
            return tenantShellAccessor.CurrentTenantShell.Value;           
        }
    }
}
