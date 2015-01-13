using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Symitar;
using Symitar.Interfaces;
using Visor.Net.Ftp;
using Visor.Options;
using File = Symitar.File;

namespace Visor.Lib
{
    public class SymDirectory
    {
        private string[] _extensionsToKeep;
        private SymSession _session;
        private ISymSocket _socket;

        public IFileSystem FileSystem { get; set; }

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

        public int Institution { get; set; }
        public string UserId { get; set; }
        public SymServerInfo Server { get; set; }

        public bool LoggedIn
        {
            get { return _session != null && _session.LoggedIn; }
        }

        private void Initialize(SymServerInfo server, int directory, string userId)
        {
            Server = server;
            Institution = directory;
            UserId = userId;

            _socket = new SymSocket(new SocketAdapter(), server.Host, server.TelnetPort);
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

            string remoteFolder = Utilities.ContainingFolder(Institution, FileUtilities.GetSymitarFileType(path));
            string remotePath = String.Format("{0}/{1}", remoteFolder, Path.GetFileNameWithoutExtension(path));

            return request.FileExists(remotePath);
        }

        public void UploadFile(string path, Action<string> successCallback, Action<FtpException> errorCallback)
        {
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            string destinationFolder = Utilities.ContainingFolder(Institution, FileUtilities.GetSymitarFileType(path));

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
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);
            var fileType = FileUtilities.GetSymitarFileType(path);
            var sourceFolder = Utilities.ContainingFolder(Institution, fileType);
            
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
            var file = FileUtilities.GetSymitarFile(path);

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

        public void Run(string fileName, List<string> promptAnswers, SymSession.FileRunStatus progressHandler,
                        RunWorkerCompletedEventHandler jobCompletionHandler)
        {
            var file = new Symitar.File {Name = fileName, Type = FileType.RepGen};
            int currPrompt = 0;

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

            List<int> sequences = _session.GetReportSequences(batchOutputSequence);
            List<string> titles = _session.GetReportTitles(batchOutputSequence);

            var reports = new List<Report>();

            for (int i = 0; i < sequences.Count(); i++)
            {
                reports.Add(new Report(FileSystem) {Sequence = sequences[i], Title = titles[i]});
            }

            return reports;
        }

        public string GetReportTitle(int sequence)
        {
            return "";
        }
    }

    public static class FileUtilities
    {
        static readonly List<string> ExtensionsToKeep = new List<string> { ".html" };

        public static File GetSymitarFile(string path)
        {
            return new File
            {
                Name = GetSymitarFileName(path),
                Type = GetSymitarFileType(path)
            };
        }

        public static string GetSymitarFileName(string path)
        {
            var extension = Path.GetExtension(path);

            if (ExtensionsToKeep.Contains(extension))
                return Path.GetFileName(path);
            else
                return Path.GetFileNameWithoutExtension(path);
        }

        public static FileType GetSymitarFileType(string path)
        {
            var extension = Path.GetExtension(path);

            switch (extension)
            {
                case ".rg":
                    return FileType.RepGen;
                case ".hlp":
                    return FileType.Help;
                case ".rpt":
                    return FileType.Report;
                default:
                    return FileType.Letter;
            }
        }
    }
}