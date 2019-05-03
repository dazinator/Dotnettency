using System.Web;

namespace Dotnettency.SystemWeb
{
    public class HttpContextWrapper : HttpContextBase
    {
        private readonly HttpContext _context;

        public HttpContextWrapper(HttpContext context)
        {
            this._context = context;
            this.Request = new Dotnettency.SystemWeb.HttpRequestWrapper(context.Request);
        }

        public override RequestBase Request { get; }
    }
}
