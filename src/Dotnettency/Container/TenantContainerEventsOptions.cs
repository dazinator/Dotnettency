using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerEventsOptions<TTenant>
       where TTenant : class
    {            

        public TenantContainerEventsOptions(ContainerBuilderOptions<TTenant> parentOptions)
        {
            ParentOptions = parentOptions;          
            TenantContainerCreatedCallbacks = new List<Action<Task<TTenant>, IServiceProvider>>();
            NestedTenantContainerCreatedCallbacks = new List<Action<Task<TTenant>, IServiceProvider>>();
        }

        public ContainerBuilderOptions<TTenant> ParentOptions { get; set; }       

        public List<Action<Task<TTenant>, IServiceProvider>> TenantContainerCreatedCallbacks { get; set; }
        public List<Action<Task<TTenant>, IServiceProvider>> NestedTenantContainerCreatedCallbacks { get; set; }

        public TenantContainerEventsOptions<TTenant> OnTenantContainerCreated(Action<Task<TTenant>, IServiceProvider> callback)
        {
            TenantContainerCreatedCallbacks.Add(callback);
            return this;
        }

        public TenantContainerEventsOptions<TTenant> OnNestedTenantContainerCreated(Action<Task<TTenant>, IServiceProvider> callback)
        {
            NestedTenantContainerCreatedCallbacks.Add(callback);
            return this;
        }
     
    }
}