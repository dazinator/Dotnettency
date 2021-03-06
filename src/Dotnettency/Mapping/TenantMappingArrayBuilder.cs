﻿using System;
using System.Collections.Generic;

namespace Dotnettency.Mapping
{
    public class TenantMappingArrayBuilder<TKey>
    {
        private List<TenantMapping<TKey>> _list = new List<TenantMapping<TKey>>();

        public void Add(TKey key, Action<TenantMapping<TKey>> configurePatterns)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            configurePatterns(mapping);
            _list.Add(mapping);
        }

        public void Add(TKey key, params string[] patterns)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            mapping.Patterns = patterns;
            _list.Add(mapping);
        }

        public void Add(TKey key, string[] patterns, string conditionName, bool requiredValue)
        {
            var mapping = new TenantMapping<TKey>();
            mapping.Key = key;
            mapping.Patterns = patterns;
            mapping.Condition = new TenantMappingEnabledCondition() { Name = conditionName, RequiredValue = requiredValue };
            _list.Add(mapping);
        }

        public TenantMapping<TKey>[] Build()
        {
            return _list.ToArray();
        }

    }
}
