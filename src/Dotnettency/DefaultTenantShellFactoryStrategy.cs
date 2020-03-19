using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    ///// <summary>
    ///// The default implementation of <see cref="ITenantShellFactoryStrategy<TTenant>"/> always selects the <see cref="ITenantShellFactory<TTenant>"/> that has been registered with the <see cref="IServiceProvider"/>
    ///// </summary>
    ///// <typeparam name="TTenant"></typeparam>
    //public class DefaultTenantShellFactoryStrategy<TTenant> : ITenantShellFactoryStrategy<TTenant>
    //     where TTenant : class
    //{
    //    private readonly IServiceProvider _serviceProvider;

    //    public DefaultTenantShellFactoryStrategy(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }
    //    public ITenantShellFactory<TTenant> GetTenantShellFactory(TenantIdentifier identifier)
    //    {
    //        return _serviceProvider.GetRequiredService<ITenantShellFactory<TTenant>>();
    //    }
    //}

    ///// <summary>
    ///// An implementation of <see cref="ITenantShellFactoryStrategy{TTenant}"/> that uses a <see cref="Func{T, TResult}"/> to select which <see cref="ITenantShellFactory{TTenant}"/> to use to load the tenant shell at request time.
    ///// </summary>
    ///// <typeparam name="TTenant"></typeparam>
    //public class DelegateTenantShellFactoryStrategy<TTenant> : ITenantShellFactoryStrategy<TTenant>
    //     where TTenant : class
    //{
    //    private readonly IServiceProvider _serviceProvider;
    //    private readonly Func<IServiceProvider, ITenantShellFactory<TTenant>> _strategyFunc;

    //    public DelegateTenantShellFactoryStrategy(IServiceProvider serviceProvider, Func<IServiceProvider, ITenantShellFactory<TTenant>> strategyFunc)
    //    {
    //        _serviceProvider = serviceProvider;
    //        _strategyFunc = strategyFunc;
    //    }
    //    public ITenantShellFactory<TTenant> GetTenantShellFactory(TenantIdentifier identifier)
    //    {
    //        return _strategyFunc(_serviceProvider);
    //    }
    //}


    ///// <summary>
    ///// An implementation of <see cref="ITenantShellFactoryStrategy{TTenant}"/> that uses a <see cref="Func{TDependency, Type}"/> to select the Type of <see cref="ITenantShellFactory{TTenant}"/> that should be Activated using DI and used to load the tenant shell at request time.
    ///// </summary>
    ///// <typeparam name="TTenant"></typeparam>
    //public class SelectTypeTenantShellFactoryStrategy<TDependency, TTenant> : ITenantShellFactoryStrategy<TTenant>
    //     where TTenant : class
    //{
    //    private readonly IServiceProvider _serviceProvider;
    //    private readonly Func<TDependency, Type> _selectTypeFunc;

    //    public SelectTypeTenantShellFactoryStrategy(IServiceProvider serviceProvider, Func<TDependency, Type> selectTypeFunc)
    //    {
    //        _serviceProvider = serviceProvider;
    //        _selectTypeFunc = selectTypeFunc;
    //    }
    //    public ITenantShellFactory<TTenant> GetTenantShellFactory(TenantIdentifier identifier)
    //    {
    //        var dep = _serviceProvider.GetRequiredService<TDependency>();
    //        var type = _selectTypeFunc?.Invoke(dep);
    //        var activated = ActivatorUtilities.CreateInstance(_serviceProvider, type) as ITenantShellFactory<TTenant>;
    //        if (activated == null)
    //        {
    //            throw new InvalidOperationException($"could not activate type: {type.FullName}");
    //        }
    //        return activated;
    //    }
    //}

    ///// <summary>
    ///// An implementation of <see cref="ITenantShellFactoryStrategy{TTenant}"/> that uses a <see cref="Func{TKey, Type}"/> to select the Type of <see cref="ITenantShellFactory{TTenant}"/> that should be Activated using DI and used to load the tenant shell at request time.
    ///// </summary>
    ///// <typeparam name="TTenant"></typeparam>
    //public class SelectTenantShellTypeFromKeyFactoryStrategy<TKey, TTenant> : ITenantShellFactoryStrategy<TTenant>
    //     where TTenant : class
    //{
    //    private readonly IServiceProvider _serviceProvider;
    //    private readonly ITenantShellFactory<TTenant> _defaultFactory;
    //    private readonly Func<TKey, Type> _selectTypeFunc;

    //    public SelectTenantShellTypeFromKeyFactoryStrategy(IServiceProvider serviceProvider, ITenantShellFactory<TTenant> defaultFactory, Func<TKey, Type> selectTypeFunc)
    //    {
    //        _serviceProvider = serviceProvider;
    //        _defaultFactory = defaultFactory;
    //        _selectTypeFunc = selectTypeFunc;
    //    }
    //    public ITenantShellFactory<TTenant> GetTenantShellFactory(TenantIdentifier identifier)
    //    {
    //        identifier.TryGetMappedTenantKey<TKey>(out TKey value);
    //        var type = _selectTypeFunc(value);
    //        if(type == null)
    //        {
    //            return _defaultFactory;
    //        }

    //        var activated = ActivatorUtilities.CreateInstance(_serviceProvider, type) as ITenantShellFactory<TTenant>;
    //        if (activated == null)
    //        {
    //            throw new InvalidOperationException($"could not activate type: {type.FullName}");
    //        }
    //        return activated;
    //    }
    //}

}
