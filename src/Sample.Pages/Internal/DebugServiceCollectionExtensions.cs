using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sample.AspNetCore30.RazorPages.Internal
{
    public static class DebugServiceCollectionExtensions
    {       

        /// <summary>
        /// Adds services for pages to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
        /// <remarks>
        /// <para>
        /// This method configures the MVC services for the commonly used features for pages. This
        /// combines the effects of <see cref="MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)"/>,
        /// <see cref="MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)"/>,
        /// <see cref="MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)"/>,
        /// <see cref="TagHelperServicesExtensions.AddCacheTagHelper(IMvcCoreBuilder)"/>,
        /// and <see cref="MvcRazorPagesMvcCoreBuilderExtensions.AddRazorPages(IMvcCoreBuilder)"/>.
        /// </para>
        /// <para>
        /// To add services for controllers for APIs call <see cref="AddControllers(IServiceCollection)"/>.
        /// </para>
        /// <para>
        /// To add services for controllers with views call <see cref="AddControllersWithViews(IServiceCollection)"/>.
        /// </para>
        /// </remarks>
        public static IMvcBuilder AddRazorPagesDebug(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = AddRazorPagesCore(services);
            return new MvcBuilder(builder.Services, builder.PartManager);
        }

        /// <summary>
        /// Adds services for pages to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configure">An <see cref="Action{MvcOptions}"/> to configure the provided <see cref="MvcOptions"/>.</param>
        /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
        /// <remarks>
        /// <para>
        /// This method configures the MVC services for the commonly used features for pages. This
        /// combines the effects of <see cref="MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)"/>,
        /// <see cref="MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)"/>,
        /// <see cref="MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)"/>,
        /// <see cref="TagHelperServicesExtensions.AddCacheTagHelper(IMvcCoreBuilder)"/>,
        /// and <see cref="MvcRazorPagesMvcCoreBuilderExtensions.AddRazorPages(IMvcCoreBuilder)"/>.
        /// </para>
        /// <para>
        /// To add services for controllers for APIs call <see cref="AddControllers(IServiceCollection)"/>.
        /// </para>
        /// <para>
        /// To add services for controllers with views call <see cref="AddControllersWithViews(IServiceCollection)"/>.
        /// </para>
        /// </remarks>
        public static IMvcBuilder AddRazorPages(this IServiceCollection services, Action<RazorPagesOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = AddRazorPagesCore(services);
            if (configure != null)
            {
                builder.AddRazorPages(configure);
            }

            return new MvcBuilder(builder.Services, builder.PartManager);

        }

        private static IMvcCoreBuilder AddRazorPagesCore(IServiceCollection services)
        {
            // This method includes the minimal things controllers need. It's not really feasible to exclude the services
            // for controllers.
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IActionDescriptorProvider, PageActionDescriptorProvider>());


            var builder = services
                .AddMvcCore()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddRazorPages()
                .AddCacheTagHelper();


            //
            // Endpoint Routing / Endpoints
            //
           // services.TryAddSingleton<ControllerActionEndpointDataSource>();
            services.TryAddSingleton<ActionEndpointFactory>();
           // services.TryAddSingleton<DynamicControllerEndpointSelector>();
           // services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, DynamicControllerEndpointMatcherPolicy>());

            services.TryAddScoped<PageActionEndpointDataSource>();
            services.AddSingleton<IActionDescriptorCollectionProvider, DefaultActionDescriptorCollectionProvider>();
            services.AddSingleton<IPageRouteModelProvider, CompiledPageRouteModelProvider>();


            AddTagHelpersFrameworkParts(builder.PartManager);

            return builder;
        }

        internal static void AddTagHelpersFrameworkParts(ApplicationPartManager partManager)
        {
            var mvcTagHelpersAssembly = typeof(InputTagHelper).GetTypeInfo().Assembly;
            if (!partManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == mvcTagHelpersAssembly))
            {
                partManager.ApplicationParts.Add(new FrameworkAssemblyPart(mvcTagHelpersAssembly));
            }

            var mvcRazorAssembly = typeof(UrlResolutionTagHelper).GetTypeInfo().Assembly;
            if (!partManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == mvcRazorAssembly))
            {
                partManager.ApplicationParts.Add(new FrameworkAssemblyPart(mvcRazorAssembly));
            }
        }

        [DebuggerDisplay("{Name}")]
        private class FrameworkAssemblyPart : AssemblyPart, ICompilationReferencesProvider
        {
            public FrameworkAssemblyPart(Assembly assembly)
                : base(assembly)
            {
            }

            IEnumerable<string> ICompilationReferencesProvider.GetReferencePaths() => Enumerable.Empty<string>();
        }

    }
}
