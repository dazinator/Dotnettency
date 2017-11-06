using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantContainerBuilder<TTenant>
    {
        Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant);
    }
}
