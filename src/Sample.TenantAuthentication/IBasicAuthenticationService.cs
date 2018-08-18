using System.Threading.Tasks;

namespace Sample.TenantAuthentication
{
    public partial class Startup
    {
        public interface IBasicAuthenticationService
        {
            Task<bool> IsValidUserAsync(string user, string password);
        }
    }
}
