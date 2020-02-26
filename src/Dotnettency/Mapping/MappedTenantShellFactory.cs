using System;
using System.Threading.Tasks;

namespace Dotnettency
{

    public abstract class MappedTenantShellFactory<TTenant, TKey> : ITenantShellFactory<TTenant>
        where TTenant : class
    {
        private readonly TenantShell<TTenant> _defaultNullShell = default(TenantShell<TTenant>);
        private readonly Task<TTenant> _defaultNullTenant = Task.FromResult(default(TTenant));


        public async Task<TenantShell<TTenant>> Get(TenantIdentifier identifier)
        {
            // This method only gets invoked once when the tenant shell needs to be created as it's not in the cache, so although performance is important, this is not a critical path.
            // this is only example code at this point:
            var keyString = identifier.Uri.PathAndQuery; // this will be a string representation of TKey.
            TTenant tenant = null;
            if (NoMappedTenantKey(keyString)) // probably use a null or special string value to indicate no tenant key matched.
            {               
                tenant = await GetDefaultTenant(identifier);
            }
            else
            {
                TKey tenantKey = (TKey)Convert.ChangeType(keyString.Substring(1), typeof(TKey));
                tenant = await GetTenant(tenantKey);
            }           

            var shell = GetTenantShell(identifier, tenant);
            return shell;
        }

        private bool NoMappedTenantKey(string keyString)
        {
            return string.IsNullOrWhiteSpace(keyString) || keyString == "/";
        }

        protected abstract Task<TTenant> GetTenant(TKey key);

        protected virtual Task<TTenant> GetDefaultTenant(TenantIdentifier key)
        {
            return _defaultNullTenant;
        }

        protected virtual TenantShell<TTenant> GetTenantShell(TenantIdentifier identifier, TTenant tenant)
        {
            if (tenant == null)
            {
                // don't return a tenant shell for a null tenant as this could be source of bugs.
                // if a catch-all shell is needed, developer can map a pattern like ** as the last mapping mapped to a tenant key XYZ,
                // and then use that key to return a default TTenant instance.
                return _defaultNullShell;
            }
            return new TenantShell<TTenant>(tenant); 
        }
    }
}