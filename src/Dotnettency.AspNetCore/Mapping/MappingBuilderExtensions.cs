using Dotnettency.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MappingBuilderExtensions
    {
      
        public static MapRequestOptionsBuilder<TTenant, TKey> Map<TTenant, TKey>(this MapRequestOptionsBuilder<TTenant, TKey> builder, Action<TenantMappingOptions<TKey>> configure)
            where TTenant : class
        {
            builder.Services.Configure<TenantMappingOptions<TKey>>(configure);

            return builder;
        }

        public static MapRequestOptionsBuilder<TTenant, TKey> WithMapping<TTenant, TKey>(this MapRequestOptionsBuilder<TTenant, TKey> builder, Action<TenantMappingArrayBuilder<TKey>> configure)
           where TTenant : class
        {
            builder.Services.Configure<TenantMappingOptions<TKey>>((a)=>a.Build(configure));
            return builder;
        }     
    }
}