using ChatClient.Net.IO;
using System.Net.Sockets;

namespace multigame.Net
{
    class server
    {
        TcpClient _tcpClient;
        public PacketReader _PacketReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;
        public event Action StartGame;
        public event Action WhoStartGame;
        public event Action Makemove;
        public event Action ReturnToQueue;
        public server() { 
            _tcpClient = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if(!_tcpClient.Connected)
            {
                _tcpClient.Connect("127.0.0.1", 7891);
                _PacketReader = new PacketReader(_tcpClient.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    PacketBuilder connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteString(username);
                    _tcpClient.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();
            }
        }
        public void SendReadyToStartGame()
        {
            PacketBuilder packetbuilder=new PacketBuilder();
            packetbuilder.WriteOpCode(20);
            _tcpClient.Client.Send(packetbuilder.GetPacketBytes());
        }
        public void SendMessageToServer(string message)
        {
            PacketBuilder packetBuilder = new PacketBuilder();
            packetBuilder.WriteOpCode(5);
            packetBuilder.WriteString(message);
            _tcpClient.Client.Send(packetBuilder.GetPacketBytes());
        }
        public void SendGameStateToServer(GameState gameState)
        {
            PacketBuilder packetBuilder = new PacketBuilder();
            packetBuilder.WriteOpCode(10);
            packetBuilder.WriteInteger(gameState.board.LastMove.x);
            packetBuilder.WriteInteger(gameState.board.LastMove.y);
            _tcpClient.Client.Send(packetBuilder.GetPacketBytes());

        }

        public void SendWinOrLoseInfoToServer(GameEndsOption EndType)
        {
            PacketBuilder packetBuilder = new PacketBuilder();
            packetBuilder.WriteOpCode(15);
            if(EndType==GameEndsOption.win) packetBuilder.WriteInteger(1);
            else if(EndType==GameEndsOption.lose) packetBuilder.WriteInteger(0);
            _tcpClient.Client.Send(packetBuilder.GetPacketBytes());
        }
        public void TESTEVENT()
        {
            PacketBuilder packetBuilder = new PacketBuilder();
            packetBuilder.WriteOpCode(5);
            packetBuilder.WriteString("TEST");
            _tcpClient.Client.Send(packetBuilder.GetPacketBytes());
        }

        public void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode=_PacketReader.ReadByte();
                    switch(opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke(); 
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        case 15:
                            StartGame?.Invoke();
                            break;
                        case 20:
                            Makemove?.Invoke();
                            break;
                        case 25:
                            WhoStartGame?.Invoke();
                            break;
                        case 40:
                            ReturnToQueue?.Invoke();
                            break;
                        default:
                            Console.WriteLine("ah yes..");
                            break;
                    }
                }

            });
        }
    }
}
