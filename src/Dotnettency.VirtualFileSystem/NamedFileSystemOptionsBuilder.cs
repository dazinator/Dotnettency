using System;

namespace Dotnettency
{
    public class NamedFileSystemOptionsBuilder<TTenant, ICabinet>
      where TTenant : class
    {
        public NamedFileSystemOptionsBuilder(NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> parent)
        {
            Parent = parent;
        }

        public NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> Parent { get; private set; }
    }

}
