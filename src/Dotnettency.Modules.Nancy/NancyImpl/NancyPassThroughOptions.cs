using System;
using Nancy;
using System.Linq;

namespace Dotnettency.Modules.Nancy
{
    /// <summary>
    /// Extensions for the NancyOptions class.
    /// </summary>
    public static class NancyPassThroughOptions
    {
        /// <summary>
        /// Tells the NancyMiddleware to pass through when
        /// response has one of the given status codes.
        /// </summary>
        /// <param name="nancyOptions">The Nancy options.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public static Func<NancyContext, bool> PassThroughWhenStatusCodesAre(params global::Nancy.HttpStatusCode[] httpStatusCode)
        {
            Func<NancyContext, bool> func = context => httpStatusCode.Any(code => context.Response.StatusCode == code);
            return func;
        }
    }
}
