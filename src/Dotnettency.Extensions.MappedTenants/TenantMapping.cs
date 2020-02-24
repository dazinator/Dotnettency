using System.Collections.Generic;

namespace Dotnettency.Extensions.MappedTenants
{
    public class TenantMapping<TKey>
    {
        public TKey Key { get; set; }
        public string[] Patterns { get; set; }
    }
}
