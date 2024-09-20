using System.Net.Sockets;
using System.Net;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using ChatServer.Net.IO;

namespace Server
{
    class Program
    {

        static TcpListener _listener;
        static List<Client> _users;

        static void Main(string[] args)
        {
            _users= new List<Client>(); 
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                BroadCastConnection();
            }
        }

        static void BroadCastConnection()
        {
            foreach(Client client in _users)
            {
                foreach(Client client2 in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(client2.username);
                    broadcastPacket.WriteMessage(client2.UID.ToString());
                    client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadCastMessage(string msg)
        {
            foreach(Client client in _users)
            {
                var BroadCastPacket = new PacketBuilder();
                BroadCastPacket.WriteOpCode(5);
                BroadCastPacket.WriteMessage(msg);
                client.ClientSocket.Client.Send(BroadCastPacket.GetPacketBytes());
            }
        }
    }
}