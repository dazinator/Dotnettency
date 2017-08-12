using Nancy;

namespace Dotnettency
{
    public class NancyModuleShellOptionsBuilder<TModule>
         // where TTenant : class
         where TModule : INancyModule
    {

        private ModuleShellOptions _moduleShellOptions = new ModuleShellOptions();

        public NancyModuleShellOptionsBuilder(TModule module)
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
