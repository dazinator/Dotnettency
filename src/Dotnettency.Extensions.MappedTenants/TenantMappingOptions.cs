using System.Collections.Generic;

namespace Dotnettency.Extensions.MappedTenants
{
    public class TenantMappingOptions<TKey>
    {
      public  TenantMapping<TKey>[] TenantMappings { get; set; }
    }
}
