using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Symitar;
using Symitar.Interfaces;
using Visor.Net.Ftp;
using Visor.ReportRunner;

namespace Visor.Options
{
    public class SymDirectory
    {
        public int Institution { get; set; }
        public string UserId { get; set; }
        public SymServerInfo Server { get; set; }

        private SymSession _session;

        private string[] ExtensionsToKeep;
        private ISymSocket _socket;

        public bool LoggedIn
        {
            get { return _session != null && _session.LoggedIn; }
        }

        public SymDirectory()
        {
            Initialize(new SymServerInfo(), 999, "");
        }

        public SymDirectory(SymServerInfo server)
        {
            Initialize(server, 999, "");
        }

        public SymDirectory(SymServerInfo server, int directory, string userId)
        {
            Initialize(server, directory, userId);
        }

        private void Initialize(SymServerInfo server, int directory, string userId)
        {
            Server = server;
            Institution = directory;
            UserId = userId;

            _socket = new SymSocket(new SocketAdapter(), server.Host, server.TelnetPort);

            ExtensionsToKeep = new string[] { ".html" };
        }

        public override string ToString()
        {
            return String.Format("Sym {0} ({1})", Institution, Server.Host);
        }

        public void Connect()
        {
            _session = new SymSession(_socket, Institution);
            _session.Connect(Server.Host, Server.TelnetPort);
            if (!_session.Login(Server.AixUsername, Server.AixPassword, UserId))
            {
                throw new Exception(_session.Error);
            }
        }

        public void Disconnect()
        {
            if (_session != null)
            {
                _session.Disconnect();
                _session = null;
            }
        }

        private void Reset()
        {
            Disconnect();
            Connect();
        }

        public bool FileExists(string path)
        {
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var remoteFolder = Utilities.ContainingFolder(Institution, GetSymitarFileType(path));
            var remotePath = String.Format("{0}/{1}", remoteFolder, Path.GetFileNameWithoutExtension(path));

            return request.FileExists(remotePath);
        }

        public void UploadFile(string path, Action<string> successCallback, Action<FtpException> errorCallback)
        {
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var destinationFolder = Utilities.ContainingFolder(Institution, GetSymitarFileType(path));

            request.Upload(
                path,
                String.Format("{0}/{1}", destinationFolder, Path.GetFileNameWithoutExtension(path)),
                successCallback,
                errorCallback,
                FtpTransferType.Text
            );
        }

        public void DownloadFile(string path, Action<string> successCallback, Action<FtpException> errorCallback)
        {
            FtpRequest request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var sourceFolder = Utilities.ContainingFolder(Institution, GetSymitarFileType(path));

            request.Download(
                String.Format("{0}/{1}", sourceFolder, Path.GetFileNameWithoutExtension(path)),
                path,                
                successCallback,
                errorCallback,
                FtpTransferType.Text
            );
        }

        public void Install(string path, Action<string, int> successCallback, Action<string, string> errorCallback)
        {
            Symitar.File file = GetSymitarFile(path);
            SpecfileResult result;

            try
            {
                if (!_session.LoggedIn)
                    Reset();

                result = _session.FileInstall(file);
            }
            catch (Exception e)
            {
                errorCallback(file.Name, e.Message);
                return;
            }

            if (result.PassedCheck)
            {
                successCallback(file.Name, result.InstallSize);
            }
            else
            {
                errorCallback(file.Name, result.ErrorMessage);
            }
        }

        private Symitar.File GetSymitarFile(string path)
        {
            return new Symitar.File
                {
                    Name = GetSymitarFileName(path), 
                    Type = GetSymitarFileType(path)
                };
        }

        private string GetSymitarFileName(string path)
        {
            var extension = Path.GetExtension(path);

            if (ExtensionsToKeep.Contains(extension))
                return Path.GetFileName(path);
            else
                return Path.GetFileNameWithoutExtension(path);
        }

        private FileType GetSymitarFileType(string path)
        {
            var extension = Path.GetExtension(path);

            switch (extension)
            {
                case ".rg": return FileType.RepGen;
                case ".hlp": return FileType.Help;
                case ".rpt": return FileType.Report;
                default: return FileType.Letter;
            }
        }

        public void Run(string fileName, List<string> promptAnswers, SymSession.FileRunStatus progressHandler, RunWorkerCompletedEventHandler jobCompletionHandler)
        {
            var file = new Symitar.File() { Name = fileName, Type = FileType.RepGen };
            var currPrompt = 0;

            if (!_session.LoggedIn)
                Reset();

            _session.FileRun(file,
                progressHandler, 
                (prompt) => currPrompt < promptAnswers.Count ? promptAnswers[currPrompt++] : "", 
                3,
                jobCompletionHandler);
        }

        public List<int> GetReportSequences(int batchOutputSequence)
        {
            return _session.GetReportSequences(batchOutputSequence);
        }

        public List<Report> GetReports(int batchOutputSequence)
        {
            if (!_session.LoggedIn)
                Reset();

            var sequences = _session.GetReportSequences(batchOutputSequence);
            var titles = _session.GetReportTitles(batchOutputSequence);

            var reports = new List<Report>();

            for (var i = 0; i < sequences.Count(); i++)
            {
                reports.Add(new Report { Sequence = sequences[i], Title = titles[i] });
            }

            return reports;
        }

        public string GetReportTitle(int sequence)
        {
            return "";
        }
    }
}
