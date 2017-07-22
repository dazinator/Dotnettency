using System;

namespace Sample
{
    public class SomeTenantService
    {
        public SomeTenantService()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
    }
}
