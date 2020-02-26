using Dotnettency.Mapping;
using System;

namespace Dotnettency
{
    public static class TenantMappingOptionsExtensions
    {
        public static TenantMappingOptions<TKey> Build<TKey>(this TenantMappingOptions<TKey> options, Action<TenantMappingArrayBuilder<TKey>> configure)
        {
            var mappingBuilder = new TenantMappingArrayBuilder<TKey>();
            configure?.Invoke(mappingBuilder);
            options.Mappings = mappingBuilder.Build();
            return options;
        }
    }
}
