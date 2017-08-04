using Dotnettency.Modules;

namespace Dotnettency
{
    public class ModuleShellOptionsBuilder<TModule>
         // where TTenant : class
         where TModule : IModule
    {

        private ModuleShellOptions _moduleShellOptions = new ModuleShellOptions();

        public ModuleShellOptionsBuilder(TModule module)
        {
            Module = module;
            //   Tenant = tenant;
        }

        //public ModuleShellOptionsBuilder<TModule> UseIsolatedContainer()
        //{
        //    _moduleShellOptions.IsRoutedModule = true;
        //    return this;
        //}

        public TModule Module { get; set; }

        ////  public TTenant Tenant { get; set; }

        internal ModuleShellOptions Build()
        {
            return _moduleShellOptions;

        }

    }

}
