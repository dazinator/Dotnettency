using DotNet.Cabinets;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace Dotnettency.HostingEnvironment
{
    public class TenantFileSystemBuilderContext<TTenant>
        where TTenant : class
    {
        private IFileProvider _parentFileProvider;

        public TTenant Tenant { get; set; }
        public Guid PartitionId { get; set; }
        public string BaseFolder { get; set; }

        public TenantFileSystemBuilderContext(TTenant tenant, string defaultTenantsBaseFolder)
        {
            Tenant = tenant;
            BaseFolder = defaultTenantsBaseFolder;
        }
        
        public TenantFileSystemBuilderContext<TTenant> AllowAccessTo(IFileProvider chainedFileProvider)
        {
            _parentFileProvider = chainedFileProvider;
            return this;
        }

        public TenantFileSystemBuilderContext<TTenant> TenantPartitionId(Guid guid)
        {
            PartitionId = guid;
            return this;
        }

        public ICabinet Build()
        {
            // Base folder needs to exist. This is the folder where the tenant specific folder will be created within.
            if (!Directory.Exists(BaseFolder))
            {
                Directory.CreateDirectory(BaseFolder);
            }

            var cabinetStorage = new PhysicalFileStorageProvider(BaseFolder, PartitionId);
            return new Cabinet(cabinetStorage, _parentFileProvider);
        }
    }
}
