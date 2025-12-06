using System.Net;
using System.Net.Sockets;

using Server;
using Server.Net.IO;

namespace MessengerServer
{
    class Program
    {
        static private List<Client> _users;
        static private TcpListener _listener;

        static void Main(string[] args)
        {
            _users = new List<Client>();

            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7878);
            _listener.Start();

            while (true)
            {
                _users.Add(new Client(_listener.AcceptTcpClient()));

                Console.WriteLine("Client Connected");

                BroadcastConnect();
            }
        }

        static void BroadcastConnect()
        {
            // send everyone the updated user list
            foreach (var userTo in _users)
            {
                // send each user's details
                foreach (var user in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(user.UID.ToString());
                    broadcastPacket.WriteMessage(user.Username);

                    userTo.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastDisconnect(Client userFrom)
        {
            foreach (var userTo in _users)
            {
                if (userTo.UID != userFrom.UID)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(10);
                    broadcastPacket.WriteMessage(userFrom.UID.ToString());
                    broadcastPacket.WriteMessage(userFrom.Username);

                    userTo.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }

            _users.Remove(userFrom);
        }

        public static void BroadcastMessage(Client userFrom, string message)
        {
            foreach (var userTo in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(5);
                broadcastPacket.WriteMessage(userFrom.UID.ToString());
                broadcastPacket.WriteMessage(userFrom.Username);
                broadcastPacket.WriteMessage(message);

                userTo.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
    }
}