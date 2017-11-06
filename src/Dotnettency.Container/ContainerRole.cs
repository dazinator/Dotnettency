namespace Dotnettency.Container
{
    public enum ContainerRole
    {
        /// <summary>
        /// The root application level container.
        /// </summary>
        Root = 0,
        /// <summary>
        /// A child container, derived from a parent.
        /// </summary>
        Child = 1,
        /// <summary>
        /// A child container that is scoped for some lifetime such as a request.
        /// </summary>
        Scoped = 2
    }
}
