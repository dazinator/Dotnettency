using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dotnettency
{
    public class TenantShell<TTenant>
        where TTenant : class
    {
        public TenantShell(TTenant tenant, params TenantDistinguisher[] distinguishers)
        {
            Id = Guid.NewGuid();
            Tenant = tenant;
            Properties = new ConcurrentDictionary<string, object>();
            Distinguishers = new HashSet<TenantDistinguisher>();

            if (distinguishers != null)
            {
                foreach (var item in distinguishers)
                {
                    Distinguishers.Add(item);
                }
            }
        }

        public ConcurrentDictionary<string, object> Properties { get; private set; }

        /// <summary>
        /// Uniquely identifies this tenant.
        /// </summary>
        public Guid Id { get; set; }

        public TTenant Tenant { get; set; }

        /// <summary>
        /// Represents context distinguihers for this same tenant. Allows future request with any of these distinguishers to be mapped to this same tenant.
        /// </summary>
        internal HashSet<TenantDistinguisher> Distinguishers { get; set; }
    }
}
