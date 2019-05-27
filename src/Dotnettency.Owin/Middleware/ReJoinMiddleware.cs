using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Dotnettency.Owin
{
    /// <summary>
    /// Middleware that accepts a root / parent pipeline delegate (not next middleware delegate) and
    /// invokes it, thus bieng the terminal middleware in the current pipeline but re-joining execution back to the root delegate / pipeline.
    /// </summary>
    public class ReJoinMiddleware
    {

        readonly Func<IDictionary<string, object>, Task> _root;

        public ReJoinMiddleware(AppFunc next, AppFunc root)
        {
            _root = root;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await _root?.Invoke(environment);
        }
    }
}