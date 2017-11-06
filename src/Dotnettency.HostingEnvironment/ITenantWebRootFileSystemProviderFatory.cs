using DotNet.Cabinets;

namespace Dotnettency.HostingEnvironment
{
    public interface ITenantWebRootFileSystemProviderFatory<TTenant>
        where TTenant : class
    {
        ICabinet GetWebRoot(TTenant tenant);
    }
}
