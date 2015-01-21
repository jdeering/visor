using System;
using System.IO;
using AlexPilotti.FTPS.Common;
using Visor.Lib.Net;

namespace Visor.Net.Ftp
{
    public class FtpRequest
    {
        private const FtpTransferType DEFAULT_TRANSFER_TYPE = FtpTransferType.Binary;
        private readonly IConnectionInformation _connectionInformation;
        private readonly ILogin _login;

        public FtpRequest(string server, int port, string username, string password)
        {
            _connectionInformation = new ConnectionInformation(server, port);
            _login = new Login(username, password);
        }

        public FtpRequest(IConnectionInformation connectionInformation, ILogin login)
        {
            _connectionInformation = connectionInformation;
            _login = login;
        }

        public string Upload(string source, string destination)
        {
            return Upload(_connectionInformation, _login, source, destination, DEFAULT_TRANSFER_TYPE);
        }

        public string Upload(string source, string destination, FtpTransferType type)
        {
            return Upload(_connectionInformation, _login, source, destination, type);
        }

        public string Upload(IConnectionInformation connectionInfo, ILogin login, string source, string destination, FtpTransferType type)
        {
            try
            {
                var client = new FtpClient(connectionInfo, login)
                {
                    TransferMode = type == FtpTransferType.Binary ? ETransferMode.Binary : ETransferMode.ASCII
                };

                client.Upload(source, destination);

                try
                {
                    client.SendCommand(String.Format("SITE CHMOD 755 {0}", destination));
                }
                catch { /* eat errors on the CHMOD command, probably already has correct permissions if failing */ }

                return Path.GetFileNameWithoutExtension(source);
            }
            catch (Exception e)
            {
                throw new FtpException(e.Message);
            }
        }

        public string Download(string source, string destination)
        {
            return Download(_connectionInformation, _login, source, destination, DEFAULT_TRANSFER_TYPE);
        }

        public string Download(string source, string destination, FtpTransferType type)
        {
            return Download(_connectionInformation, _login, source, destination, type);
        }

        public string Download(IConnectionInformation connectionInfo, ILogin login, string source, string destination, FtpTransferType type)
        {
            try
            {
                var client = new FtpClient(connectionInfo, login);
                client.TransferMode = type == FtpTransferType.Binary ? ETransferMode.Binary : ETransferMode.ASCII;

                var successful = client.Download(source, destination);
                return successful ? Path.GetFileNameWithoutExtension(source) : "";
            }
            catch (Exception e)
            {
                throw new FtpException(e.Message);
            }
        }

        public bool FileExists(string path)
        {
            var client = new FtpClient(_connectionInformation, _login);
            return client.FileExists(path);
        }
    }

    public class FtpException : Exception
    {
        public FtpException() : base()
        {
        }

        public FtpException(string message)
            : base(message)
        {
        }

        public FtpException(string message, Exception e) : base(message, e)
        {
        }
    }
}