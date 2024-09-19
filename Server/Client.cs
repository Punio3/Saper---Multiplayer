using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public TcpClient ClientSocket { get; set; }
        public Guid UID { get; set; }

        public Client(TcpClient tcpClient)
        {
            ClientSocket = tcpClient;
            UID = Guid.NewGuid();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with username");
        }

    }
}
