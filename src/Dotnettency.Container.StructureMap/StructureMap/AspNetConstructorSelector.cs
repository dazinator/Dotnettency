/// Taken from https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection/blob/0d20c416c5423f5153a71589f5082a9b234b123c/src/StructureMap.Microsoft.DependencyInjection/HelperExtensions.cs
/// Licenced under MIT Licence.
/// With changes by Darrell Tunnell.
/// 
using Dotnettency.Container.StructureMap.StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;
using System;
using System.Linq;
using System.Reflection;

internal class AspNetConstructorSelector : IConstructorSelector
{
    // ASP.NET expects registered services to be considered when selecting a ctor, SM doesn't by default.
    public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
    {
        var typeInfo = pluggedType.GetTypeInfo();
        var constructors = typeInfo.DeclaredConstructors;

        //MvcRouteHandler
        ConstructorInfo chosenConstructor;

        if (typeInfo.Name == "MvcRouteHandler")
        {
            var chosenCtor = constructors
           .Where(PublicConstructors)
           .Select(ctor => new Holder(ctor))
            .ToArray().OrderBy(x => x.Order).FirstOrDefault();
            chosenConstructor = chosenCtor.Constructor;           
        }


        var publicConstructors = constructors
            .Where(PublicConstructors)
            .Select(ctor => new Holder(ctor))
            .ToArray();


        var validConstructors = publicConstructors
            .Where(x => x.CanSatisfy(dependencies, graph))
            .ToArray();

        chosenConstructor = validConstructors
            .OrderByDescending(x => x.Order)
            .Select(x => x.Constructor)
            .FirstOrDefault();

        return chosenConstructor;
    }

    private static bool PublicConstructors(ConstructorInfo constructor)
    {
        // IsConstructor is false for static constructors.
        return constructor.IsConstructor && !constructor.IsPrivate;
    }

    private struct Holder
    {
        public Holder(ConstructorInfo constructor)
        {
            Constructor = constructor;
            Parameters = constructor.GetParameters();
        }

        public ConstructorInfo Constructor { get; }

        public int Order => Parameters.Length;

        private ParameterInfo[] Parameters { get; }

        public bool CanSatisfy(DependencyCollection dependencies, PluginGraph graph)
        {
            foreach (var parameter in Parameters)
            {
                var type = parameter.ParameterType;

                if (type.IsGenericEnumerable())
                {
                    // Because graph.HasFamily returns false for IEnumerable<T>,
                    // we unwrap the generic argument and pass that instead.
                    type = type.GenericTypeArguments[0];
                }

                if (graph.HasFamily(type))
                {
                    continue;
                }

                if (dependencies.Any(dep => dep.Type == type))
                {
                    continue;
                }



                return false;
            }

            return true;
        }
    }
}