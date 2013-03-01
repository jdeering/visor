﻿using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Symitar;
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

        public bool LoggedIn
        {
            get { return _session.LoggedIn; }
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

            ExtensionsToKeep = new string[] { ".html" };
        }

        public override string ToString()
        {
            return String.Format("Sym {0} ({1})", Institution, Server.Host);
        }

        public void Connect()
        {
            _session = new SymSession(Institution);
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

        public bool FileExists(string path)
        {
            FileType type = GetSymitarFileType(path);
            string fileName = GetSymitarFileName(path);

            return _session.FileExists(fileName, type);
        }

        public void UploadFile(string path, Action<string> SuccessCallback, Action<FtpException> ErrorCallback)
        {
            FtpRequest request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var destinationFolder = Utilities.ContainingFolder(Institution, GetSymitarFileType(path));

            request.Upload(
                path,
                String.Format("{0}/{1}", destinationFolder, Path.GetFileNameWithoutExtension(path)),
                SuccessCallback,
                ErrorCallback,
                FtpTransferType.Text
            );
        }

        public void DownloadFile(string path, Action<string> SuccessCallback, Action<FtpException> ErrorCallback)
        {
            FtpRequest request = new FtpRequest(Server.Host, Server.FtpPort, Server.AixUsername, Server.AixPassword);

            var sourceFolder = Utilities.ContainingFolder(Institution, GetSymitarFileType(path));

            request.Download(
                String.Format("{0}/{1}", sourceFolder, Path.GetFileNameWithoutExtension(path)),
                path,                
                SuccessCallback,
                ErrorCallback,
                FtpTransferType.Text
            );
        }

        public void Install(string path, Action<string, int> SuccessCallback, Action<string, string> ErrorCallback)
        {
            Symitar.File file = GetSymitarFile(path);
            SpecfileResult result;

            try
            {
                if (!_session.LoggedIn)
                {
                    Disconnect();
                    Connect();
                }

                result = _session.FileInstall(file);
            }
            catch (Exception e)
            {
                ErrorCallback(file.Name, e.Message);
                return;
            }

            if (result.PassedCheck)
            {
                SuccessCallback(file.Name, result.InstallSize);
            }
            else
            {
                ErrorCallback(file.Name, result.ErrorMessage);
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

        public void Run(string fileName, SymSession.FileRunStatus ProgressHandler, RunWorkerCompletedEventHandler JobCompletionHandler)
        {
            var file = new Symitar.File() { Name = fileName, Type = FileType.RepGen };

            _session.FileRun(file,
                ProgressHandler, 
                GetPromptValue, 
                3,
                JobCompletionHandler);
        }

        public List<int> GetReportSequences(int batchOutputSequence)
        {
            return _session.GetReportSequences(batchOutputSequence);
        }

        public List<Report> GetReports(int batchOutputSequence)
        {
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

        private string GetPromptValue(string prompt)
        {
            return "";
        }
    }
}
