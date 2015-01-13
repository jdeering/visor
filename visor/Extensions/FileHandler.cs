using System;
using System.IO;
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
                var dte = Package.GetGlobalService(typeof (DTE)) as DTE;
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
                var dte = Package.GetGlobalService(typeof (DTE)) as DTE;
                string path = String.Format("{0}{1}.rpt", Path.GetTempPath(), fileName);

                dte.ExecuteCommand("File.OpenFile", path);
            }
            catch (Exception)
            {
            }
        }
    }
}