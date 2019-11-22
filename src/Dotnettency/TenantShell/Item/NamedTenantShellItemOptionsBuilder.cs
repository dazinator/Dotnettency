using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class NamedTenantShellItemOptionsBuilder<TTenant, TTItem>
        where TTenant : class
    {
        private readonly IServiceCollection _services;
        private Dictionary<string, ITenantShellItemFactory<TTenant, TTItem>> _namedFactories;
        //  var delegateFact = new DelegateTenantShellItemFactory<TTenant, TItem>(fact);
        public NamedTenantShellItemOptionsBuilder(MultitenancyOptionsBuilder<TTenant> parent)
        {
            Parent = parent;
            _services = parent.Services;
            _namedFactories = new Dictionary<string, ITenantShellItemFactory<TTenant, TTItem>>();
        }

      //  public MultitenancyOptionsBuilder<TTenant> ParentBuilder { get; private set; }

        public NamedTenantShellItemOptionsBuilder<TTenant, TTItem> Add(string name, Func<TenantShellItemBuilderContext<TTenant>, TTItem> configureItem)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(name);
            }

            var factory = new DelegateTenantShellItemFactory<TTenant, TTItem>(configureItem);
            _namedFactories.Add(name, factory);
            // optionsBuilder.Services.AddSingleton<ITenantShellItemFactory<TTenant, TTItem>>(factory);    
            return this;
        }

        public MultitenancyOptionsBuilder<TTenant> Parent { get; set; }
        internal void Build()
        {
            _services.TryAddScoped<ITenantShellNamedItemAccessor<TTenant, TTItem>>((sp) =>
            {
                var shellAccessor = sp.GetRequiredService<ITenantShellAccessor<TTenant>>();
                return new TenantShellNamedItemAccessor<TTenant, TTItem>(shellAccessor, _namedFactories);
            });

            // For named Items, we add support injection of Func<string, Task<TItem>> - a convenience that allows non blocking access to shell item registered with the specified name.
            // minor contention on Lazy<>
            _services.AddScoped(sp =>
            {
                Func<string, Task<TTItem>> factoryFunc = (s) =>
                {
                    var namedAccessor = sp.GetRequiredService<ITenantShellNamedItemAccessor<TTenant, TTItem>>();
                    var lazy = namedAccessor.NamedFactory(sp, s);
                    return lazy.Value;
                };
                return factoryFunc;
            });
        }
    }


}
