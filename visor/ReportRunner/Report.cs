using System.IO;
using Visor.Extensions;

namespace Visor.ReportRunner
{
    public class Report
    {
        public BatchJob Parent { get; set; }
        public string Title { get; set; }
        public int Sequence { get; set; }

        public void Open()
        {
            string fileName = string.Format("{0}.rpt", Sequence.ToString().PadLeft(6, '0'));

            Parent.Directory.DownloadFile(Path.GetTempPath() + fileName, FileHandler.OpenReportFile, null);
        }
    }
}