/// Taken from https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection/
/// Licenced under MIT Licence.
/// With changes by Darrell Tunnell.
/// 
using Dotnettency.Container.StructureMap.StructureMap;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace Dotnettency.Container.StructureMap
{
    public class StructureMapServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private IContainer _container;

        public StructureMapServiceProvider(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsGenericEnumerable())
            {
                // Ideally we'd like to call TryGetInstance here as well,
                // but StructureMap does't like it for some weird reason.
                return GetRequiredService(serviceType);
            }

            return _container.TryGetInstance(serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}
