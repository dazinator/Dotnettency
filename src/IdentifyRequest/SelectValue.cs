using Microsoft.AspNetCore.Http;

namespace IdentifyRequest
{
    public delegate string SelectValue(HttpContext httpContext);
}
