namespace Dotnettency
{
    public static class MappingBuilderExtensions
    {
        public static MapRequestOptionsBuilder<TTenant, TKey> UsingDotNetGlobPatternMatching<TTenant, TKey>(this MapRequestOptionsBuilder<TTenant, TKey> builder)
            where TTenant : class
        {
            builder.SetPatternMatcherFactory<DotNetGlobTenantMatcherFactory<TKey>>();
            return builder;
        }      
    }
}