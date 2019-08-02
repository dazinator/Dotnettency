using System;

namespace Dotnettency.Container
{
    public class TenantContainerBuilderContext<TTenant>
    {
        public TTenant Tenant { get; set; }

        public IServiceProvider ApplicationServices { get; set; }
    }
}
