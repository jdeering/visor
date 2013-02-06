﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symitar;
using Visor.Net.Ftp;

namespace Visor.Options
{
    public class SymDirectory
    {
        public int Institution { get; set; }
        public string UserId { get; set; }
        public SymServerInfo Server { get; set; }

        private SymSession _session;

        private string[] ExtensionsToKeep;

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
            _session.Login(Server.AixUsername, Server.AixPassword, UserId);
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

        private FileType GetSymitarFileType(string path)
        {
            var extension = Path.GetExtension(path);

            switch (extension)
            {
                case ".rg": return FileType.RepGen;
                case ".hlp": return FileType.Help;
                default: return FileType.Letter;
            }
        }

        private string GetSymitarFileName(string path)
        {
            var extension = Path.GetExtension(path);

            if (ExtensionsToKeep.Contains(extension))
                return Path.GetFileName(path);
            else
                return Path.GetFileNameWithoutExtension(path);
        }
    }
}
