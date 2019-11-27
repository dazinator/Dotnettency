using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantContainerBuilder<TTenant>
        where TTenant : class
    {
        Task<ITenantContainerAdaptor> BuildAsync(TenantShellItemBuilderContext<TTenant> tenantContext);
    }
}
