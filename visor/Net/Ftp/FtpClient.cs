using System.Net;
using AlexPilotti.FTPS.Client;
using AlexPilotti.FTPS.Common;

namespace Visor.Net.Ftp
{
    public class FtpClient
    {
        private IConnectionInformation _connection;
        private ILogin _login;
        private ETransferMode _transferMode;

        public IConnectionInformation ConnectionInformation
        {
            get { return _connection; }
            set { _connection = value;  }
        }
        public ILogin Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public ETransferMode TransferMode
        {
            get { return _transferMode; }
            set { _transferMode = value; }
        }

        public FtpClient(IConnectionInformation connectionInfo, ILogin login)
        {
            _connection = connectionInfo;
            _login = login;
        }

        public bool Download(string source, string destination)
        {
            ulong bytesRead;
            var credential = new NetworkCredential(_login.Username, _login.Password);
            using (var client = new FTPSClient())
            {
                client.Connect(_connection.Server, _connection.Port, credential, ESSLSupportMode.ClearText, null, null, 0, 0, 0, null);
                client.SetTransferMode(_transferMode);
                // Download a file
                bytesRead = client.GetFile(source, destination);
            }

            return bytesRead > 0;
        }

        public bool Upload(string source, string destination)
        {
            ulong bytesPut;
            var credential = new NetworkCredential(_login.Username, _login.Password);
            using (var client = new FTPSClient())
            {
                client.Connect(_connection.Server, _connection.Port, credential, ESSLSupportMode.ClearText, null, null, 0, 0, 0, null);
                client.SetTransferMode(_transferMode);
                // Upload a file
                bytesPut = client.PutFile(source, destination);
            }

            return bytesPut > 0;
        }

        public void SendCommand(string command)
        {
            var credential = new NetworkCredential(_login.Username, _login.Password);
            using (var client = new FTPSClient())
            {
                client.Connect(_connection.Server, _connection.Port, credential, ESSLSupportMode.ClearText, null, null, 0, 0, 0, null);
                client.SendCustomCommand(command);
            }
        }

        public bool FileExists(string path)
        {
            var credential = new NetworkCredential(_login.Username, _login.Password);
            using (var client = new FTPSClient())
            {
                client.Connect(_connection.Server, _connection.Port, credential, ESSLSupportMode.ClearText, null, null, 0, 0, 0, null);
                return client.GetFile(path) != null;
            }
        }
    }
}
