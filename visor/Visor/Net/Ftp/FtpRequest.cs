﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visor.Net.Ftp
{
    public class FtpRequest
    {
        private const FtpTransferType DEFAULT_TRANSFER_TYPE = FtpTransferType.Binary;
        private IConnectionInformation _connectionInformation;
        private ILogin _login;

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

        /*
         * FILE UPLOAD
         * 
         */
        public void Upload(string source, string destination, Action<string> SuccessCallback)
        {
            Upload(_connectionInformation, _login, source, destination, SuccessCallback, null, DEFAULT_TRANSFER_TYPE);
        }

        public void Upload(string source, string destination, Action<string> SuccessCallback, FtpTransferType type)
        {
            Upload(_connectionInformation, _login, source, destination, SuccessCallback, null, type);
        }

        public void Upload(string source, string destination, Action<string> SuccessCallback, Action<FtpException> ErrorCallback, FtpTransferType type)
        {
            Upload(_connectionInformation, _login, source, destination, SuccessCallback, ErrorCallback, type);
        }

        public void Upload(IConnectionInformation connectionInfo, ILogin login, string source, string destination, Action<string> SuccessCallback, Action<FtpException> ErrorCallback, FtpTransferType type)
        {
            try
            {
                FtpClient client = new FtpClient(connectionInfo, login);

                if (type == FtpTransferType.Binary)
                    client.TransferMode = AlexPilotti.FTPS.Common.ETransferMode.Binary;
                else
                    client.TransferMode = AlexPilotti.FTPS.Common.ETransferMode.ASCII;

                client.Upload(source, destination);

                try { client.SendCommand(String.Format("SITE CHMOD 755 {0}", destination)); } 
                catch { /* eat errors on the CHMOD command, probably already has correct permissions if failing */ }
            }
            catch (Exception e)
            {
                if (ErrorCallback != null)
                    ErrorCallback(new FtpException(e.Message));
                return;
            }
            SuccessCallback(Path.GetFileNameWithoutExtension(source));
        }

        /*
         * FILE DOWNLOAD
         * 
         */
        public void Download(string source, string destination, Action<string> SuccessCallback)
        {
            Download(_connectionInformation, _login, source, destination, SuccessCallback, null, DEFAULT_TRANSFER_TYPE);
        }

        public void Download(string source, string destination, Action<string> SuccessCallback, FtpTransferType type)
        {
            Download(_connectionInformation, _login, source, destination, SuccessCallback, null, type);
        }

        public void Download(string source, string destination, Action<string> SuccessCallback, Action<FtpException> ErrorCallback, FtpTransferType type)
        {
            Download(_connectionInformation, _login, source, destination, SuccessCallback, ErrorCallback, type);
        }

        public void Download(IConnectionInformation connectionInfo, ILogin login, string source, string destination, Action<string> SuccessCallback, Action<FtpException> ErrorCallback, FtpTransferType type)
        {
            try
            {
                FtpClient client = new FtpClient(connectionInfo, login);

                if (type == FtpTransferType.Binary)
                    client.TransferMode = AlexPilotti.FTPS.Common.ETransferMode.Binary;
                else
                    client.TransferMode = AlexPilotti.FTPS.Common.ETransferMode.ASCII;

                client.Download(source, destination);
            }
            catch (Exception e)
            {
                if (ErrorCallback != null)
                    ErrorCallback(new FtpException(e.Message));
                return;
            }
            SuccessCallback(Path.GetFileNameWithoutExtension(destination));
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
