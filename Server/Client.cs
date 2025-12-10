using Server.Net.IO;
using System.Net.Sockets;

using MessengerServer;

namespace Server
{
    class Client {
        public Guid UID { get; set; }
        public string Username { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());
            _packetReader.ReadByte();
            
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}][{UID}]: Client Connected: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opCode = _packetReader.ReadByte();

                    switch (opCode)
                    {
                        case 5:
                            var username = _packetReader.ReadMessage();
                            var message = _packetReader.ReadMessage();

                            Console.WriteLine($"[{DateTime.Now}][{UID}]: Message Received: {message}");

                            Program.SendMessageToUser(this, username, message);

                            break;

                        default:
                            Console.WriteLine("Unsupported OpCode");

                            break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine($"[{DateTime.Now}][{UID}]: Client Disconnected");

                    Program.BroadcastDisconnect(this);

                    ClientSocket.Close();

                    break;
                }
            }
        }
    }
}
