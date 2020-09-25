using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnettency.Owin
{
    public class HttpRequestWrapper : RequestBase
    {
        private readonly IReadOnlyDictionary<string, object> _environment;

        private const string SchemeDelimiter = "://";

        public HttpRequestWrapper(IReadOnlyDictionary<string, object> environment)
        {
            this._environment = environment;
        }

        /// <summary>
        /// Returns the combined components of the request URL as a URI.
        /// </summary>
        /// <param name="environment">The owin environment.</param>
        /// <returns></returns>
        public static Uri GetUri(IReadOnlyDictionary<string, object> environment)
        {


            var headers = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];
            //   requestHeaders = 
            var host = headers["Host"][0];
            var pathBase = (string)environment["owin.RequestPathBase"];
            var path = (string)environment["owin.RequestPath"];
            var queryString = (string)environment["owin.RequestQueryString"];
            var scheme = (string)environment["owin.RequestScheme"];

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new Uri(new StringBuilder(length)
                .Append(scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString());
        }

        public override Uri GetUri()
        {
            return GetUri(_environment);
        }
    }
}