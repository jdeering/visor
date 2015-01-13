using Microsoft.VisualStudio.Shell;

namespace Visor.ReportRunner
{
    internal sealed class ReportToolWindow : ToolWindowPane
    {
        private readonly ReportRunnerControl _control;

        public ReportToolWindow() : base(null)
        {
            Caption = "Generated Reports";
            _control = new ReportRunnerControl();
            Content = _control;
        }
    }
}