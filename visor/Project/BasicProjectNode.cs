using System;
using Microsoft.VisualStudio.Project;

namespace Visor.Project
{
    public class BasicProjectNode : ProjectNode
    {
        private VisorPackage _package;

        public BasicProjectNode(VisorPackage package)
        {
            _package = package;
        }

        public override Guid ProjectGuid
        {
            get { return GuidList.VisorProjectFactory; }
        }

        public override string ProjectType
        {
            get { return "BasicProjectType"; }
        }

        public override void AddFileFromTemplate(string source, string target)
        {
            FileTemplateProcessor.UntokenFile(source, target);
            FileTemplateProcessor.Reset();
        }
    }
}