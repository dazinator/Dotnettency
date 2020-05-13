using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace IdentifyRequest
{
    public static class HttpRequestExtensions
    {
        private const string SchemeDelimiter = "://";

        /// <summary>
        /// Returns the combined components of the request URL as a URI.
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns></returns>
        public static Uri GetUri(this HttpRequest request)
        {
            var host = request.Host.Value;
            var pathBase = request.PathBase.Value;
            var path = request.Path.Value;
            var queryString = request.QueryString.Value;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = request.Scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new Uri(new StringBuilder(length)
                .Append(request.Scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString());
        }

        ///// <summary>
        ///// Returns the combined components of the request URL authority.
        ///// </summary>
        ///// <param name="request">The request to assemble the uri pieces from.</param>
        ///// <returns></returns>
        //public static Uri GetAuthorityUri(this HttpRequest request)
        //{
        //    var host = request.Host.Value;
        //    var pathBase = request.PathBase.Value;
        //    var queryString = request.QueryString.Value;

        //    // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
        //    var length = request.Scheme.Length + SchemeDelimiter.Length + host.Length
        //        + pathBase.Length;

        //    return new Uri(new StringBuilder(length)
        //        .Append(request.Scheme)
        //        .Append(SchemeDelimiter)
        //        .Append(host)
        //        .Append(pathBase)
        //        .Append(queryString)
        //        .ToString());
        //}
    }
}
