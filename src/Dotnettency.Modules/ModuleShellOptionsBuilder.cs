using Dotnettency.Modules;

namespace Dotnettency
{
    public class ModuleShellOptionsBuilder<TModule>
         // where TTenant : class
         where TModule : IModule
    {

        private ModuleShellOptions<TModule> _moduleShellOptions = new ModuleShellOptions<TModule>();

        public ModuleShellOptionsBuilder(TModule module)
        {
            Module = module;
            //   Tenant = tenant;
        }



        public ModuleShellOptionsBuilder<TModule> UseIsolatedContainer()
        {
            _moduleShellOptions.IsIsolated = true;
            return this;
        }

        public TModule Module { get; set; }

        ////  public TTenant Tenant { get; set; }

        internal ModuleShellOptions<TModule> Build()
        {
            return _moduleShellOptions;

        }

    }

}
