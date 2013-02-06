using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visor.Net;

namespace Visor.Options
{
    public class SymServerInfo
    {
        public string Host { get; set; }
        public int TelnetPort { get; set; }
        public int FtpPort { get; set; }
        public string AixUsername { get; set; }
        public string AixPassword { get; set; }
    }
}
