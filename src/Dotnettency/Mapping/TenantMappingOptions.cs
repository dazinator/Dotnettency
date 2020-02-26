using Dotnettency.Mapping;

namespace Dotnettency
{
    public class TenantMappingOptions<TKey>
    {
        public TenantMapping<TKey>[] Mappings { get; set; }
    }
}
