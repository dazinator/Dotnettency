using System;
using System.Collections.Generic;

namespace Dotnettency
{
    public class ConditionRegistry
    {

        private IDictionary<string, Func<bool>> _conditions;
        //private readonly IServiceProvider _sp;

        internal IServiceProvider ServiceProvider { get; set; }

        public ConditionRegistry()
        {
            _conditions = new Dictionary<string, Func<bool>>();
            //_sp = sp;
        }

        public void Add(string name, bool conditionValue)
        {
            var wrapped = new Func<bool>(() => conditionValue);
            _conditions.Add(name, wrapped);
        }

        public void Add(string name, Func<bool> getConditionValue)
        {
            _conditions.Add(name, getConditionValue);
        }

        public void Add(string name, Func<IServiceProvider, bool> getConditionValue)
        {
            var wrapped = new Func<bool>(() => getConditionValue(ServiceProvider));
            _conditions.Add(name, wrapped);
        }

        public Func<bool> GetEvaluateCondition(string name, bool requiredValue)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            if (!_conditions.ContainsKey(name))
            {
                return null;
            }
            var condition = _conditions[name];
            return () => condition() == requiredValue;
        }

        public bool Evaluate(string name, bool requiredValue)
        {
            var condition = _conditions[name];
            return condition() == requiredValue;
        }
    }

}
