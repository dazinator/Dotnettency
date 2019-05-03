using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore
{
    public class AspNetCoreHttpContextWrapper : HttpContextBase
    {
        private readonly HttpContext _context;

        public AspNetCoreHttpContextWrapper(HttpContext context)
        {
            this._context = context;
            this.Request = new AspNetCoreRequestWrapper(context.Request);
        }

        public override RequestBase Request { get; }
    }
}
