using System;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.AspNetCore.Http;

namespace Sample.RazorPages
{
    public class TenantMiddlewareDiagnosticListener
    {

        public Tenant Tenant { get; set; }

        public TenantMiddlewareDiagnosticListener(Tenant tenant)
        {
            Tenant = tenant;
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
        public virtual void OnMiddlewareStarting(HttpContext httpContext, string name)
        {
            WriteMessage($"{Tenant?.Name ?? "NULL"} MiddlewareStarting: {name}; {httpContext.Request.Path}");
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
        public virtual void OnMiddlewareException(Exception exception, string name)
        {
            WriteMessage($"{Tenant?.Name ?? "NULL"} MiddlewareException: {name}; {exception.Message}");
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
        public virtual void OnMiddlewareFinished(HttpContext httpContext, string name)
        {          
            WriteMessage($"{Tenant?.Name ?? "NULL"}MiddlewareFinished: {name}; {httpContext.Response.StatusCode}");           
        }

        private void WriteMessage(string message)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }
}
