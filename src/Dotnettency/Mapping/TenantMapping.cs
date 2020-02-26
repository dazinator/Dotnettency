using System.Collections.Generic;

namespace Dotnettency
{
    public class TenantMapping<TKey>
    {
        public TKey Key { get; set; }
        public string[] Patterns { get; set; }
    }
}
