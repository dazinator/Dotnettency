using System;

namespace Dotnettency.Middleware
{
    public abstract class AppBuilderAdaptorBase
    {
        public abstract void UseMiddleware<TMiddleware>();

        public abstract void UseMiddleware<TMiddleware>(params object[] args);

        public abstract IServiceProvider ApplicationServices { get; set; }

    }


}
