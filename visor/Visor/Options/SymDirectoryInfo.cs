using System;
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
            Institution = directory;
            UserId = userId;

            _session = new SymSession(Institution);
            _session.Connect(Server.Host, Server.TelnetPort);
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
    }
}
