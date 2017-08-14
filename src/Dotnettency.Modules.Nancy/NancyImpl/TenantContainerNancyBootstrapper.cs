using System;
using System.Collections.Generic;
using Dotnettency.Container;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nancy.ViewEngines;

namespace Dotnettency.Modules.Nancy
{

    public class TenantContainerNancyBootstrapper<TTenant> : NancyBootstrapperWithRequestContainerBase<ITenantContainerAdaptor>, IDisposable
     where TTenant : class
    {

        private readonly ITenantContainerAdaptor _applicationContainer;

        private bool isDisposing = false;

        public ITenantContainerAdaptor RequestContainerAdaptor { get; set; }


        //public int ITenantContainerAdaptor { get; set; }

        public TenantContainerNancyBootstrapper(ITenantContainerAdaptor applicationContainer)
        {
            _applicationContainer = applicationContainer;
        }

        #region Container Factories

        protected override ITenantContainerAdaptor GetApplicationContainer()
        {
            return _applicationContainer;
        }

        protected override ITenantContainerAdaptor CreateRequestContainer(NancyContext context)
        {
            var perRequestContainer = RequestContainerAdaptor;
            return perRequestContainer;
        }


        #endregion

        #region From ApplicationContainer

        protected override IEnumerable<INancyModule> GetAllModules(ITenantContainerAdaptor container)
        {
            var sp = this.ApplicationContainer;
            return sp.GetServices<INancyModule>();
        }

        public override INancyEnvironment GetEnvironment()
        {
            var sp = this.ApplicationContainer;
            return sp.GetService<INancyEnvironment>();
        }

        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            var sp = this.ApplicationContainer;
            return sp.GetServices<IApplicationStartup>();
        }

        protected override IDiagnostics GetDiagnostics()
        {
            var sp = ApplicationContainer;
            return sp.GetService<IDiagnostics>();
        }

        protected override INancyEngine GetEngineInternal()
        {
            var sp = ApplicationContainer;
            return sp.GetService<INancyEngine>();
        }

        protected override INancyEnvironmentConfigurator GetEnvironmentConfigurator()
        {
            var sp = ApplicationContainer;
            return sp.GetService<INancyEnvironmentConfigurator>();
        }

        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            var sp = ApplicationContainer;
            return sp.GetServices<IRegistrations>();
        }

        #endregion


        protected override INancyModule GetModule(ITenantContainerAdaptor container, Type moduleType)
        {
            var sp = container;
            return (INancyModule)sp.GetService(moduleType);
        }

        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(ITenantContainerAdaptor container, Type[] requestStartupTypes)
        {
            var sp = container;
            return requestStartupTypes.Select(sp.GetService).Cast<IRequestStartup>().ToArray();
        }

        protected override void RegisterBootstrapperTypes(ITenantContainerAdaptor applicationContainer)
        {
            applicationContainer.Configure((services) =>
            {
                services.AddSingleton<INancyModuleCatalog>(this);
                services.AddSingleton<IFileSystemReader, DefaultFileSystemReader>();
            });
        }

        protected override void RegisterCollectionTypes(ITenantContainerAdaptor container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
            container.Configure((services) =>
            {
                foreach (var collectionTypeRegistration in collectionTypeRegistrationsn)
                {
                    foreach (var implementationType in collectionTypeRegistration.ImplementationTypes)
                    {
                        RegisterType(
                            collectionTypeRegistration.RegistrationType,
                            implementationType,
                            container.Role == ContainerRole.Scoped ? Lifetime.PerRequest : collectionTypeRegistration.Lifetime,
                            services);
                    }
                }
            });
        }

        protected override void RegisterTypes(ITenantContainerAdaptor container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            container.Configure((services) =>
            {
                foreach (var typeRegistration in typeRegistrations)
                {

                    RegisterType(
                        typeRegistration.RegistrationType,
                        typeRegistration.ImplementationType,
                        container.Role == ContainerRole.Scoped ? Lifetime.PerRequest : typeRegistration.Lifetime,
                        services);
                }
            });
        }
        protected override void RegisterInstances(ITenantContainerAdaptor container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            container.Configure((services) =>
            {
                foreach (var instanceRegistration in instanceRegistrations)
                {
                    services.AddSingleton(instanceRegistration.RegistrationType, instanceRegistration.Implementation);
                }
            });
        }

        protected override void RegisterNancyEnvironment(ITenantContainerAdaptor container, INancyEnvironment environment)
        {
            container.Configure((services) =>
            {
                services.AddSingleton<INancyEnvironment>(environment);
            });
        }

        protected override void RegisterRequestContainerModules(ITenantContainerAdaptor container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            container.Configure((services) =>
            {
                foreach (var registrationType in moduleRegistrationTypes)
                {
                    services.AddTransient(typeof(INancyModule), registrationType.ModuleType);
                }
            });
        }

        #region Helper Methods

        protected void RegisterType(Type registrationType, Type implementationType, Lifetime lifetime, IServiceCollection services)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    services.AddTransient(registrationType, implementationType);
                    break;

                case Lifetime.Singleton:
                    services.AddSingleton(registrationType, implementationType);
                    break;
                case Lifetime.PerRequest:
                    services.AddScoped(registrationType, implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("lifetime", lifetime, String.Format("Unknown Lifetime: {0}.", lifetime));
            }

        }

        #endregion

        //protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        //{
        //    get
        //    {
        //        return NancyInternalConfiguration.WithOverrides(x => x.NancyModuleBuilder = typeof(CustomNancyModuleBuilder));
        //    }
        //}

        public new void Dispose()
        {
            if (this.isDisposing)
            {
                return;
            }

            this.isDisposing = true;
            base.Dispose();
        }
    }
}
