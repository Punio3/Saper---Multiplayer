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
        static bool isGameStarted;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            isGameStarted = false;
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                BroadCastConnection();

                if (_users.Count == 4 && !isGameStarted)
                {
                    Task.Run(() => StartGame());
                }
            }
        }

        static void BroadCastConnection()
        {
            foreach (Client client in _users)
            {
                foreach (Client client2 in _users)
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
            foreach (Client client in _users)
            {
                var BroadCastPacket = new PacketBuilder();
                BroadCastPacket.WriteOpCode(5);
                BroadCastPacket.WriteMessage(msg);
                client.ClientSocket.Client.Send(BroadCastPacket.GetPacketBytes());
            }
        }

        public static void BroadCastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);

            foreach (Client client in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            BroadCastMessage($"{disconnectedUser.username}: Disconnected");
        }

        public static void BroadCastGameScene()
        {
            foreach (Client client in _users)
            {
                var BroadCastPacket = new PacketBuilder();
                BroadCastPacket.WriteOpCode(15);
                client.ClientSocket.Client.Send(BroadCastPacket.GetPacketBytes());
            }
        }

        public static async Task StartGame()
        {
            int iteration = 4;

            Console.WriteLine($"{DateTime.Now}: Game starting");
            for (int k = iteration; k >= 0; k--)
            {
                BroadCastMessage("Server: Time to start game: " + k.ToString());
                await Task.Delay(1000);
            }
            isGameStarted = true;
            BroadCastGameScene();
        }
    }
}