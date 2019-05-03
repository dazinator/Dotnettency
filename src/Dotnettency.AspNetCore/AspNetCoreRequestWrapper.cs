using Microsoft.AspNetCore.Http;
using System;

namespace Dotnettency.AspNetCore
{
    public class AspNetCoreRequestWrapper : RequestBase
    {
        private readonly HttpRequest request;

        public AspNetCoreRequestWrapper(HttpRequest request)
        {
            this.request = request;           
        }

        public override Uri GetUri()
        {
            return request.GetUri();
        }
    }
}
