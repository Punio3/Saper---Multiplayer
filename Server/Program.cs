using System.Net.Sockets;
using System.Net;
using System.Collections.Specialized;

namespace Server
{
    class Program
    {

        static TcpListener _listener;
        static void Main(string[] args)
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());

            }
        }

    }
}