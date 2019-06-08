using System;

namespace Dotnettency.Container
{
    public interface ITenantContainerEventsPublisher<TTenantOrModule> : ITenantContainerEvents<TTenantOrModule>
        where TTenantOrModule : class
    {    

        void PublishTenantContainerCreated(IServiceProvider serviceProvider);

        void PublishNestedTenantContainerCreated(IServiceProvider serviceProvider);

    }
}