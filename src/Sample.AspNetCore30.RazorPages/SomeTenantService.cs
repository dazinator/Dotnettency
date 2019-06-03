using System;
using System.IO;

namespace Sample
{
    public class SomeTenantService
    {

        public SomeTenantService(Tenant tenant)
        {
            Id = Guid.NewGuid();
            TenantName = tenant?.Name;          
        }
        public Guid Id { get; set; }

        public string TenantName { get; set; }       

    }
}
