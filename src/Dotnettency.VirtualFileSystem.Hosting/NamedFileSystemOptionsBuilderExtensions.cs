using Microsoft.AspNetCore.Hosting;

namespace Dotnettency
{
    public static class NamedFileSystemOptionsBuilderExtensions
    {

        //public NamedFileSystemOptionsBuilder(NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> parent) : base(parent.ParentBuilder)
        //{
        //    _parent = parent;
        //}

        public static NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> UseAsEnvironmentContentRootFileProvider<TTenant, ICabinet>(this NamedFileSystemOptionsBuilder<TTenant, ICabinet> builder, IWebHostEnvironment environment)
        where TTenant : class
        {
            builder.Parent.Parent.UseContentVirtualFileSystemFileProvider((fp) =>
            {
                environment.ContentRootFileProvider = fp;
            });

            return builder.Parent;
        }

        public static NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> UseAsEnvironmentWebRootFileProvider<TTenant, ICabinet>(this NamedFileSystemOptionsBuilder<TTenant, ICabinet> builder, IWebHostEnvironment environment)
       where TTenant : class
        {
            builder.Parent.Parent.UseWebVirtualFileSystemFileProvider((fp) =>
            {
                environment.WebRootFileProvider = fp;
            });

            return builder.Parent;
        }


    }

}
