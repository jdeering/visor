using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Visor.Project
{
    [Guid(GuidList.VisorProjectFactoryString)]
    public class ProjectFactory : Microsoft.VisualStudio.Project.ProjectFactory
    {
        private readonly VisorPackage _package;

        public ProjectFactory(VisorPackage package) : base(package)
        {
            _package = package;
        }

        protected override ProjectNode CreateProject()
        {
            var project = new BasicProjectNode(_package);
            var provider = _package as IServiceProvider;
            project.SetSite((IOleServiceProvider) provider.GetService(typeof (IOleServiceProvider)));
            return project;
        }
    }
}