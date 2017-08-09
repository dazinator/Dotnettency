using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Nancy;
using Nancy.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    /// <summary>
    /// Bridges the communication between Nancy and ASP.NET CORE based hosting.
    /// </summary>
    public class NancyHandler
    {
        private readonly INancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyHandler"/> type for the specified <paramref name="engine"/>.
        /// </summary>
        /// <param name="engine">An <see cref="INancyEngine"/> instance, that should be used by the handler.</param>
        public NancyHandler(INancyEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>
        /// Processes the ASP.NET request with Nancy.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContextBase"/> of the request.</param>
        public async Task ProcessRequest(HttpContext httpContext, Func<NancyContext, bool> performPassThrough, RequestDelegate next)
        {
            var request = CreateNancyRequest(httpContext);
            using (var nancyContext = await this.engine.HandleRequest(request).ConfigureAwait(false))
            {
                await SetNancyResponseToHttpResponse(httpContext, nancyContext, performPassThrough, next);
            }
        }

        private async Task SetNancyResponseToHttpResponse(HttpContext httpContext, NancyContext nancyContext, Func<NancyContext, bool> performPassThrough, RequestDelegate next)
        {
            if (performPassThrough(nancyContext))
            {
                await next.Invoke(httpContext);
            }

            SetHttpResponseHeaders(httpContext, nancyContext.Response);

            if (nancyContext.Response.ContentType != null)
            {
                httpContext.Response.ContentType = nancyContext.Response.ContentType;
            }

            //if (IsOutputBufferDisabled())
            //{
            //    context.Response.BufferOutput = false;
            //}

            httpContext.Response.StatusCode = (int)nancyContext.Response.StatusCode;

            if (nancyContext.Response.ReasonPhrase != null)
            {
                httpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = nancyContext.Response.ReasonPhrase;
            }
            // response.Contents.Invoke(new NancyResponseStream(context.Response.f));            
            nancyContext.Response.Contents.Invoke(httpContext.Response.Body);
        }

        private static Request CreateNancyRequest(HttpContext context)
        {
            var incomingHeaders = context.Request.Headers.ToDictionary(a => a.Key, b => b.Value.AsEnumerable());

            var expectedRequestLength =
                GetExpectedRequestLength(incomingHeaders);

            var basePath = context.Request.PathBase.HasValue ? context.Request.PathBase.Value.TrimEnd('/') : "";
            var path = context.Request.Path.Value;

            // var path = context.Request.Url.AbsolutePath.Substring(basePath.Length);
            path = string.IsNullOrWhiteSpace(path) ? "/" : path;
            var uri = context.Request.GetUri();
            var nancyUrl = new Url
            {
                Scheme = uri.Scheme,
                HostName = uri.Host,
                Port = uri.Port,
                BasePath = basePath,
                Path = path,
                Query = uri.Query,
            };


            var clientCert = context.Connection.ClientCertificate;
            //if (clientCert != null && clientCert.RawData != null && clientCert.RawData.Length != 0)
            //{
            //   // certificate = cert.RawData;
            //}

            //using (var requestBodyStream = new MemoryStream())
            //{                
            //    var requestBody = context.Request.Body;
            //    await requestBody.CopyToAsync(requestBodyStream);
            //    requestBodyStream.Seek(0, SeekOrigin.Begin);

            //}

            RequestStream body = null;

            if (expectedRequestLength != 0)
            {
                //   requestBodyStream.Seek(0, SeekOrigin.Begin);

                body = RequestStream.FromStream(context.Request.Body, expectedRequestLength, StaticConfiguration.DisableRequestStreamSwitching ?? true);
            }

            var protocol = context.Request.Protocol;
            //  var protocolVersion = context.Request.ServerVariables["HTTP_VERSION"];
            var method = context.Request.Method;
            var remoteIp = context.Connection.RemoteIpAddress.ToString();


            return new Request(
                method.ToUpperInvariant(),
                nancyUrl,
                body,
                incomingHeaders,
                remoteIp,
                clientCert,
                protocol);
        }

        private static long GetExpectedRequestLength(IDictionary<string, IEnumerable<string>> incomingHeaders)
        {
            if (incomingHeaders == null)
            {
                return 0;
            }

            if (!incomingHeaders.ContainsKey("Content-Length"))
            {
                return 0;
            }

            var headerValue =
                incomingHeaders["Content-Length"].SingleOrDefault();

            if (headerValue == null)
            {
                return 0;
            }

            long contentLength;
            if (!long.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength))
            {
                return 0;
            }

            return contentLength;
        }

        private static void SetHttpResponseHeaders(HttpContext context, Response response)
        {
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                foreach (var header in response.Headers.ToDictionary(x => x.Key, x => x.Value))
                {
                    httpContext.Response.Headers.Add(header.Key, header.Value);
                }

                foreach (var cookie in response.Cookies.ToArray())
                {                  
                    httpContext.Response.Headers.Add("Set-Cookie", cookie.ToString());                  
                }

                return Task.FromResult(0);
            }, context);
        }
    }
}
