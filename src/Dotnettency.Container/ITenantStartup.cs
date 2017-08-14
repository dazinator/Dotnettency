using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Container
{
    public interface ITenantStartup
    {
        void ConfigureServices(IServiceCollection services);
    }


    //public class ConfigureTenantFromServicesTenantContainerFactory<TTenant> : ITenantContainerFactory<TTenant>
    //    where TTenant : class
    //{

    //    public ConfigureTenantFromServicesTenantContainerFactory()
    //    {

    //    }

    //    public Task<IServiceProvider> Get(TenantIdentifier identifier)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}




}
