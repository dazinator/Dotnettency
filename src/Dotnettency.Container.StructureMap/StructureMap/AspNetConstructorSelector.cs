/// Taken from https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection/blob/0d20c416c5423f5153a71589f5082a9b234b123c/src/StructureMap.Microsoft.DependencyInjection/HelperExtensions.cs
/// Licenced under MIT Licence.
/// With changes by Darrell Tunnell.
/// 
using StructureMap.Graph;
using StructureMap.Pipeline;
using System;
using System.Linq;
using System.Reflection;

internal class AspNetConstructorSelector : IConstructorSelector
{
    // ASP.NET expects registered services to be considered when selecting a ctor, SM doesn't by default.
    public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph) =>
        pluggedType.GetTypeInfo()
            .DeclaredConstructors
            .Where(ctor => ctor.IsConstructor && !ctor.IsPrivate) // IsConstructor is false for static constructors
            .Select(ctor => new { Constructor = ctor, Parameters = ctor.GetParameters() })
            .Where(x => x.Parameters.All(param => graph.HasFamily(param.ParameterType) || dependencies.Any(dep => dep.Type == param.ParameterType)))
            .OrderByDescending(x => x.Parameters.Length)
            .Select(x => x.Constructor)
            .FirstOrDefault();
}
