using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visor.Net
{
    public class ConnectionInformation : IConnectionInformation
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public ConnectionInformation(string server, int port)
        {
            Server = server;
            Port = port;
        }
    }
}
