namespace Visor.Net
{
    public class ConnectionInformation : IConnectionInformation
    {
        public ConnectionInformation(string server, int port)
        {
            Server = server;
            Port = port;
        }

        public string Server { get; set; }
        public int Port { get; set; }
    }
}