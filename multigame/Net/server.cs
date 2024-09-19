using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace multigame.Net
{
    class server
    {
        TcpClient _tcpClient;
        public server() { 
            _tcpClient = new TcpClient();
        }

        public void ConnectToServer()
        {
            if(!_tcpClient.Connected)
            {
                _tcpClient.Connect("127.0.0.1", 7891);
            }
        }

    }
}
