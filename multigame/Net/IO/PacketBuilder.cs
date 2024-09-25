using System.IO;
using System.Text;

namespace ChatClient.Net.IO
{
    class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        public void WriteString(string msg)
        {
            var msgLength = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLength));
            _ms.Write(Encoding.ASCII.GetBytes(msg));
        }
        public void WriteInteger(int value)
        {
            var intBytes = BitConverter.GetBytes(value);
            _ms.Write(intBytes, 0, intBytes.Length);
        }
        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }


    }
}