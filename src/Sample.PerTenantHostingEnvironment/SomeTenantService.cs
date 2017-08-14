using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Sample.PerTenantHostingEnvironment
{
    public class SomeTenantService
    {
        private readonly IHostingEnvironment _hostingEnv;

        public SomeTenantService(Tenant tenant,
            IHostingEnvironment hostingEnv)
        {
            Id = Guid.NewGuid();
            TenantName = tenant?.Name;
            _hostingEnv = hostingEnv;
        }
        public Guid Id { get; set; }

        public string TenantName { get; set; }

        public string GetContentFile(string path)
        {
            //var allFiles = _hostingEnv.ContentRootFileProvider.GetDirectoryContents("");
            //foreach (var item in allFiles)
            //{

            //}
            var file = _hostingEnv.ContentRootFileProvider.GetFileInfo(path);
            if (!file.Exists)
            {
                return string.Empty;
            }
            using (var reader = new StreamReader(file.CreateReadStream()))
            {
                var contents = reader.ReadToEnd();
                return contents;
            };
        }

        public string GetWebRootFile(string path)
        {
            //var allFiles = _hostingEnv.ContentRootFileProvider.GetDirectoryContents("");
            //foreach (var item in allFiles)
            //{

            //}
            var file = _hostingEnv.WebRootFileProvider.GetFileInfo(path);
            if (!file.Exists)
            {
                return string.Empty;
            }
            using (var reader = new StreamReader(file.CreateReadStream()))
            {
                var contents = reader.ReadToEnd();
                return contents;
            };
        }

    }
}
