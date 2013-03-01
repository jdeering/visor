using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Visor.Extensions
{
    public static class FileHandler
    {
        public static string GetActiveDocumentPath()
        {
            try
            {
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
                return dte.ActiveDocument.FullName;
            }
            catch (Exception e)
            {
                throw new Exception("There is no active document.", e);
            }
        }

        public static void OpenReportFile(string fileName)
        {
            try
            {
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
                var path = String.Format("{0}{1}.rpt", Path.GetTempPath(), fileName);

                dte.ExecuteCommand("File.OpenFile", path);
            }
            catch (Exception)
            {
            }
        }
    }
}
