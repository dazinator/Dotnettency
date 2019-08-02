using DotNet.Cabinets;

namespace Dotnettency.TenantFileSystem
{
    public interface ITenantFileSystemProviderFactory<TTenant>
                where TTenant : class
    {
        ICabinet GetCabinet(TTenant tenant);
    }
}
