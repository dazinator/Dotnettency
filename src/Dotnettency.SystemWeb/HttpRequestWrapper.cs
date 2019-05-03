using System;
using System.Web;

namespace Dotnettency.SystemWeb
{

    public class HttpRequestWrapper : RequestBase
    {
        private readonly HttpRequest request;

        public HttpRequestWrapper(HttpRequest request)
        {
            this.request = request;
        }

        public override Uri GetUri()
        {
            return request.GetUri();
        }
    }
}
