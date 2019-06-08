using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellRestarter<TTenant>
        where TTenant : class
    {
        Task Restart();
    }
}