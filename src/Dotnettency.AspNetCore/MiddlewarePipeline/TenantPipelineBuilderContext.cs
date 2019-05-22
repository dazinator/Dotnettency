namespace Dotnettency.AspNetCore.MiddlewarePipeline
{
    public class TenantPipelineBuilderContext<TTenant>
    {
        public TTenant Tenant { get; set; }
    }
}
