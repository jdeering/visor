using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Visor.ReportRunner
{
    internal sealed class ReportToolWindow : ToolWindowPane
    {
        private ReportRunnerControl _control;

        public ReportToolWindow() : base(null)
        {
            Caption = "Generated Reports";
            _control = new ReportRunnerControl();
            Content = _control;
        }
    }
}
