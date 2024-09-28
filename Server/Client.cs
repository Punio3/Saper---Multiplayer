using ChatServer.Net.IO;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public TcpClient ClientSocket { get; set; }
        public Guid UID { get; set; }
        public string username { get; set; }

        PacketReader _packetReader;
        public Client(TcpClient tcpClient)
        {
            ClientSocket = tcpClient;
            UID = Guid.NewGuid();
            _packetReader=new PacketReader(ClientSocket.GetStream());
            var opcode=_packetReader.ReadByte();
            username=_packetReader.ReadMessage();
            

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with username: {username}");

            Task.Run(() => Process());
        }

         void Process()
         {
            while (true)
            {
                try { 
                var opcode=_packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"{DateTime.Now}: Message received: {msg}");
                            Program.BroadCastMessage($"{DateTime.Now}: {username}: {msg}");
                            break;
                        case 10:
                            Point tmp=new Point(_packetReader.ReadInt(),_packetReader.ReadInt());
                            Console.WriteLine($"{DateTime.Now}: {username} make move: ({tmp.x},{tmp.y})");
                            Program.BroadCastSendGameState(new GameState(tmp));
                            break;
                        case 15:
                            int EndType=_packetReader.ReadInt();
                            if(EndType==0) Console.WriteLine($"{DateTime.Now}: {username} lost game");
                            else if(EndType==1) Console.WriteLine($"{DateTime.Now}: {username} won game");
                            Program.isGameStarted = false;
                            Program.AmountPlayersReadyToStartGame--;
                            Program.BroadCastReturnClientToQueue(UID);
                            break;
                        case 20:
                            Program.AmountPlayersReadyToStartGame++;
                            string Text= $"{Program.AmountPlayersReadyToStartGame}/{Program.AmountPlayersToStartGame}";
                            Console.WriteLine($"{DateTime.Now}: {username} is ready. " +
                            $"Ready Players ({Text}).");
                            Program.BroadCastMessage($"{DateTime.Now}: {username} is ready. Ready Players ({Text}) ");
                            if ( Program.AmountPlayersReadyToStartGame == Program.AmountPlayersToStartGame && !Program.isGameStarted)
                            {
                                Task.Run(() => Program.StartGame());
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{UID.ToString()}: Disconnected {username}");
                    Program.BroadCastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }           
            }
         }

    }
}
