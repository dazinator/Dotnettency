using Microsoft.AspNetCore.Mvc;
using System;

namespace Dotnettency.Modules
{
    public static class ModuleRegisterBuilderExtensions
    {
        public static MvcModulesBuilder<TModule> AddMvcModules<TModule>(this ModuleRegisterBuilder builder, Action<MvcOptions> mvcOptionsSetup = null)
            where TModule : class, IRoutedModule
        {
            var mvcModulesBuilder = new MvcModulesBuilder<TModule>(builder, mvcOptionsSetup);
            return mvcModulesBuilder;
        }
    }



}
