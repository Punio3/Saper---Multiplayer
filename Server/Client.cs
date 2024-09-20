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
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    
                    break;
                }           
            }
         }

    }
}
