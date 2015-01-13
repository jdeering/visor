using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Visor.Project
{
    [Guid(GuidList.VisorProjectFactoryString)]
    public class ProjectFactory : Microsoft.VisualStudio.Project.ProjectFactory
    {
        private VisorPackage _package;

        public ProjectFactory(VisorPackage package) : base(package)
        {
            _package = package;
        }

        protected override ProjectNode CreateProject()
        {
            var project = new BasicProjectNode(_package);
            var provider = _package as IServiceProvider;
            project.SetSite((IOleServiceProvider)provider.GetService(typeof(IOleServiceProvider)));
            return project;
        }
    }
}
