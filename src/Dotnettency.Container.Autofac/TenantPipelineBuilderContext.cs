using System;

namespace Dotnettency
{
    public class TenantPipelineBuilderContext<TTenant>
    {
        public TTenant Tenant { get; set; }

        public IServiceProvider ApplicationServices { get; set; }
    }
}
