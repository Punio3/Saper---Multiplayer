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
        static int WhoMoves;
        static bool isGameStarted;
        static int AmountPlayersToStartGame;
        static GameState gameState;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            AmountPlayersToStartGame = 2;
            isGameStarted = false;
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                BroadCastConnection();
                BroadCastConnectionMessage(client);

                if (_users.Count == AmountPlayersToStartGame && !isGameStarted)
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
        public static void BroadCastSendWhoStartGame(string username)
        {
            foreach (Client client in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(25);
                broadcastPacket.WriteMessage(username);
                client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
        public static void BroadCastSendGameState(GameState gamestate)
        {
            Guid LastPlayerUid = _users[WhoMoves].UID;
            WhoMoves=(WhoMoves+1)%AmountPlayersToStartGame;
            foreach(Client client in _users)
            {
                if (client.UID != LastPlayerUid)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(20);
                    broadcastPacket.WriteMessage(gamestate.LastMove.x);
                    broadcastPacket.WriteMessage(gamestate.LastMove.y);
                    broadcastPacket.WriteMessage(_users[WhoMoves].username);
                    client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
                else
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(25);
                    broadcastPacket.WriteMessage(_users[WhoMoves].username);
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

        public static void BroadCastConnectionMessage(Client client)
        {
            foreach (Client clients in _users)
            {
                if (clients != client)
                {
                    var BroadCastPacket = new PacketBuilder();
                    BroadCastPacket.WriteOpCode(5);
                    BroadCastPacket.WriteMessage($"Server: User {client.username} connected to the server. {_users.Count}/{AmountPlayersToStartGame} in queue. ");
                    clients.ClientSocket.Client.Send(BroadCastPacket.GetPacketBytes());
                }
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
            WhoMoves = 0;

            Console.WriteLine($"{DateTime.Now}: Game starting");
            for (int k = iteration; k >= 0; k--)
            {
                BroadCastMessage("Server: Time to start game: " + k.ToString());
                await Task.Delay(1000);
            }
            isGameStarted = true;
            BroadCastGameScene();
            BroadCastSendWhoStartGame(_users[WhoMoves].username);
            Console.WriteLine($"{DateTime.Now}: {_users[WhoMoves].username} starting game");
        }
    }
}