using System;

namespace Dotnettency.Extensions.MappedTenants
{
    public static class TenantMappingOptionsExtensions
    {
        public static TenantMappingOptions<TKey> Build<TKey>(this TenantMappingOptions<TKey> options, Action<TenantMappingArrayBuilder<TKey>> configure)
        {
            var mappingBuilder = new TenantMappingArrayBuilder<TKey>();
            configure?.Invoke(mappingBuilder);
            options.TenantMappings = mappingBuilder.Build();
            return options;
        }
    }
}
