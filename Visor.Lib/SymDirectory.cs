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

        public bool FileExists(string source)
        {
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var remoteFolder = Utilities.ContainingFolder(Institution, FileUtilities.GetSymitarFileType(source));
            var remotePath = String.Format("{0}/{1}", remoteFolder, Path.GetFileNameWithoutExtension(source));

            return request.FileExists(remotePath);
        }

        public string UploadFile(string source)
        {
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var destinationFolder = Utilities.ContainingFolder(Institution, FileUtilities.GetSymitarFileType(source));
            var destination = String.Format("{0}/{1}", destinationFolder, Path.GetFileNameWithoutExtension(source));

            return request.Upload(source, destination, FtpTransferType.Text);
        }

        public string DownloadFile(string destination)
        {
            var request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);
            var fileType = FileUtilities.GetSymitarFileType(destination);
            var sourceFolder = Utilities.ContainingFolder(Institution, fileType);

            var source = String.Format("{0}/{1}", sourceFolder, Path.GetFileNameWithoutExtension(destination));

            return request.Download(source, destination, FtpTransferType.Text);
        }

        public int Install(string path)
        {
            var file = FileUtilities.GetSymitarFile(path);

            if (!_session.LoggedIn) Reset();
            var result = _session.FileInstall(file);

            if (!result.PassedCheck) throw new Exception(result.ErrorMessage);
            return result.InstallSize;
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