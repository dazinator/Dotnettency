using DotNet.Cabinets;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace Dotnettency.VirtualFileSystem
{
    public class PhysicalStorageCabinetBuilder
    {
        private IFileProvider _parentFileProvider;

        /// <summary>
        /// Create a new Cabinet builder.
        /// </summary>
        /// <param name="basePhysicalPath">The root phsyical path where any physical folders will be created.
        /// Will attempt to create this directory if it doesn't already exist. </param>
        public PhysicalStorageCabinetBuilder(string basePhysicalPath)
        {
            BaseFolder = basePhysicalPath;
        }

        public Guid PartitionId { get; set; }
        protected string BaseFolder { get; set; }
        public string SubDirectory { get; set; }

        /// <summary>
        /// Chains another file provider to this tenants file system, so that the tenant can access those additional files (Read and Copy on Write access)
        /// </summary>
        /// <param name="chainedFileProvider"></param>
        /// <returns></returns>
        public PhysicalStorageCabinetBuilder AllowAccessTo(IFileProvider chainedFileProvider)
        {
            _parentFileProvider = chainedFileProvider;
            return this;
        }

        public PhysicalStorageCabinetBuilder SetPartitionId(Guid guid)
        {
            PartitionId = guid;
            return this;
        }

        /// <summary>
        /// Append a sub directory to the BaseFolder, this effects where the partition will be created.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PhysicalStorageCabinetBuilder SetSubDirectory(string subDirectory)
        {
            SubDirectory = subDirectory;
            return this;
        }

        public string GetBasePath()
        {
            if (!string.IsNullOrWhiteSpace(SubDirectory))
            {
                return Path.Combine(BaseFolder, SubDirectory);
            }
            else
            {
                return BaseFolder;
            }
        }

        public ICabinet Build()
        {
            // Base physical folder needs to exist.
            // This is the folder where the tenant specific folder will be created within.
            var basePath = GetBasePath();
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            var cabinetStorage = new PhysicalFileStorageProvider(basePath, PartitionId);
            return new Cabinet(cabinetStorage, _parentFileProvider);
        }
    }



}
