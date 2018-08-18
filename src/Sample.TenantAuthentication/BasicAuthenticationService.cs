using System.Threading.Tasks;

namespace Sample.TenantAuthentication
{
    public partial class Startup
    {
        public class BasicAuthenticationService : IBasicAuthenticationService
        {

            Task<bool> IBasicAuthenticationService.IsValidUserAsync(string user, string password)
            {
                return Task.FromResult(true);
            }
        }
    }
}
