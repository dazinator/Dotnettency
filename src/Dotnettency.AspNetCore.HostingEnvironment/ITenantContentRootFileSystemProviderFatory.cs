using DotNet.Cabinets;

namespace Dotnettency.AspNetCore.HostingEnvironment
{
    public interface ITenantContentRootFileSystemProviderFactory<TTenant>
        where TTenant : class
    {
        ICabinet GetContentRoot(TTenant tenant);
    }
}
