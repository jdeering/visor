using System.IO;
using Symitar;
using Visor.Extensions;

namespace Visor.Lib
{
    public class Report
    {
        public BatchJob Parent { get; set; }
        public string Title { get; set; }
        public int Sequence { get; set; }

        public Report()
        {
            
        }

        public Report(IFileSystem fileHandler)
        {
            FileHandler = fileHandler;
        }

        protected IFileSystem FileHandler { get; set; }

        public void Open()
        {
            string fileName = string.Format("{0}.rpt", Sequence.ToString().PadLeft(6, '0'));
            Parent.Directory.DownloadFile(Path.GetTempPath() + fileName);
        }

        private void OpenReport(string path)
        {
            FileHandler.Open(FileType.Report, path);
        }
    }
}