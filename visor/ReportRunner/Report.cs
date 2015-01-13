using System.IO;
using Microsoft.VisualStudio.Shell;
using Visor.Extensions;
using Visor.Net.Ftp;
using Visor.Options;

namespace Visor.ReportRunner
{
    public class Report
    {
        public BatchJob Parent { get; set; }
        public string Title { get; set; }
        public int Sequence { get; set; }

        public void Open()
        {
            var fileName = string.Format("{0}.rpt", Sequence.ToString().PadLeft(6, '0'));

            Parent.Directory.DownloadFile(Path.GetTempPath()+fileName, FileHandler.OpenReportFile, null);
        }
    }
}