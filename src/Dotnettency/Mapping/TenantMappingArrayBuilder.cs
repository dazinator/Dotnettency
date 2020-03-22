using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Dotnettency.Mapping
{
    public class TenantMappingArrayBuilder<TKey>
    {
        private List<TenantMapping<TKey>> _list = new List<TenantMapping<TKey>>();

        public TenantMappingArrayBuilder<TKey> Add(TKey key, string factoryName, string[] patterns)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            mapping.FactoryName = factoryName;
            mapping.Patterns = patterns;
            _list.Add(mapping);
            return this;
        }

        public TenantMappingArrayBuilder<TKey> Add(TKey key, params string[] patterns)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            mapping.Patterns = patterns;
            _list.Add(mapping);
            return this;
        }
        public TenantMappingArrayBuilder<TKey> Add(TKey key, string[] patterns, string conditionName, bool requiredValue = true)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            mapping.Patterns = patterns;
            mapping.Condition = new TenantMappingEnabledCondition() { Name = conditionName, RequiredValue = requiredValue };
            _list.Add(mapping);
            return this;
        }

        public TenantMappingArrayBuilder<TKey> Add(TKey key, string[] patterns, string conditionName, bool requiredValue, string factoryName)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            mapping.Patterns = patterns;
            mapping.Condition = new TenantMappingEnabledCondition() { Name = conditionName, RequiredValue = requiredValue };
            mapping.FactoryName = factoryName;
            _list.Add(mapping);
            return this;
        }

        public TenantMapping<TKey>[] Build()
        {
            return _list.ToArray();
        }

    }
}
