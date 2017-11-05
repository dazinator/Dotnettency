using DotNet.Cabinets;

namespace Dotnettency.HostingEnvironment
{
    public interface ITenantContentRootFileSystemProviderFatory<TTenant>
        where TTenant : class
    {
        ICabinet GetContentRoot(TTenant tenant);
    }
}
