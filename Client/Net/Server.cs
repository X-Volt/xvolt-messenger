using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using Client.Net.IO;

namespace Client.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader? PacketReader;

        public event Action? ConnectEvent;
        public event Action? DisconnectEvent;
        public event Action? MessageReceivedEvent;

        public Server()
        {
            _client = new TcpClient();
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (PacketReader != null)
                {
                    var opCode = PacketReader.ReadByte();

                    switch (opCode)
                    {
                        case 1:
                            ConnectEvent?.Invoke();
                            break;

                        case 5:
                            MessageReceivedEvent?.Invoke();
                            break;

                        case 10:
                            DisconnectEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine("Unsupported OpCode");
                            break;
                    }
                }
            });
        }

        public void SignOn(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7878);

                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);

                    _client.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();
            }
        }

        public void SendMessageToUser(string username, string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(username);
            messagePacket.WriteMessage(message);

            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
