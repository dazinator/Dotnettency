using System;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.AspNetCore.Http;

namespace Sample.RazorPages
{
    public class ApplicationMiddlewareDiagnosticListener
    {

        public ApplicationMiddlewareDiagnosticListener()
        {

        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
        public virtual void OnMiddlewareStarting(HttpContext httpContext, string name)
        {
            WriteMessage($"Application MiddlewareStarting: {name}; {httpContext.Request.Path}");
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
        public virtual void OnMiddlewareException(Exception exception, string name)
        {
            WriteMessage($"Application MiddlewareException: {name}; {exception.Message}");
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
        public virtual void OnMiddlewareFinished(HttpContext httpContext, string name)
        {
            WriteMessage($"Application MiddlewareFinished: {name}; {httpContext.Response.StatusCode}");
        }

        private void WriteMessage(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }

    }
}
