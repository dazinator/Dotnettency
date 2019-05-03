using System.Collections.Generic;

namespace Dotnettency.Owin
{
    public class HttpContextWrapper : HttpContextBase
    {
        public HttpContextWrapper(IReadOnlyDictionary<string, object> owinEnvironment)
        {
            Request = new HttpRequestWrapper(owinEnvironment);
        }
        public override RequestBase Request { get; }
    }

}