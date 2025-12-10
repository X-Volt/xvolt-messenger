using System.Net;
using System.Net.Sockets;

using Server;
using Server.Net.IO;

namespace MessengerServer
{
    class Program
    {
        static private IDictionary<string, Client> _users;
        static private TcpListener? _listener;

        static void Main(string[] args)
        {
            _users = new Dictionary<string, Client>();

            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7878);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client.Username, client);

                Console.WriteLine("Client Connected");

                BroadcastConnect();
            }
        }

        public static void BroadcastConnect()
        {
            // TODO: get this to only send to the user's active chat pages

            // send everyone the updated user list
            foreach (var userTo in _users.Values)
            {
                // send each user's details
                foreach (var user in _users.Values)
                {
                    var packet = new PacketBuilder();
                    packet.WriteOpCode(1);
                    packet.WriteMessage(user.UID.ToString());
                    packet.WriteMessage(user.Username);

                    userTo.ClientSocket.Client.Send(packet.GetPacketBytes());
                }
            }
        }

        public static void BroadcastDisconnect(Client userFrom)
        {
            // TODO: get this to only send to the user's active chat pages

            foreach (var userTo in _users.Values)
            {
                if (userTo.UID != userFrom.UID)
                {
                    var packet = new PacketBuilder();
                    packet.WriteOpCode(10);
                    packet.WriteMessage(userFrom.UID.ToString());
                    packet.WriteMessage(userFrom.Username);

                    userTo.ClientSocket.Client.Send(packet.GetPacketBytes());
                }
            }

            _users.Remove(userFrom.Username);
        }

        public static void SendMessageToUser(Client userFrom, string usernameTo, string message)
        {
            // Add support for offline messaging ...
            // - store the date in the message, on the server side, so we know when it was sent
            // - send the message right away when the recipient is online (no server storage)
            // - when they are not online, check if they are a registered user ...
            // - if not, then ignore, otherwise store the message temporarily on the server
            // - when the user connects, send them their offline messages and clear the storage
            
            foreach (var userTo in _users.Values)
            {
                if (userTo.Username == usernameTo)
                {
                    var packet = new PacketBuilder();
                    packet.WriteOpCode(5);
                    packet.WriteMessage(userFrom.UID.ToString());
                    packet.WriteMessage(userFrom.Username);
                    packet.WriteMessage(userTo.Username);
                    packet.WriteMessage(message);

                    if (userFrom.UID != userTo.UID)
                    {
                        userFrom.ClientSocket.Client.Send(packet.GetPacketBytes());
                    }
                    
                    userTo.ClientSocket.Client.Send(packet.GetPacketBytes());

                    break;
                }
            }
        }
    }
}