using DotNet.Cabinets;

namespace Dotnettency.AspNetCore.HostingEnvironment
{
    public interface ITenantWebRootFileSystemProviderFactory<TTenant>
        where TTenant : class
    {
        ICabinet GetWebRoot(TTenant tenant);
    }
}
