using DotNet.Cabinets;

namespace Dotnettency.TenantFileSystem
{
    public interface ITenantFileSystemProviderFactory<TTenant>
                where TTenant : class
    {
        ICabinet GetRoot(TTenant tenant);
    }
}
