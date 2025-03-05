using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using System.Diagnostics;
using IgniteView.Core;
using System.Collections.Concurrent;

namespace Oberon.Remote.Desktop
{
    public class SocketServer
    {
        WebSocketServer Server;

        SynchronizedCollection<IWebSocketConnection> Connections = new SynchronizedCollection<IWebSocketConnection>();

        [Command("forwardInput")]
        public static void ForwardInput(string input)
        {
            var bytes = Convert.FromBase64String(input);
            var server = Program.SocketServer.Server;

            if (server != null) 
            {
                foreach (var socket in Program.SocketServer.Connections)
                {
                    if (socket.IsAvailable)
                    {
                        socket.Send(bytes);
                    }
                }
            }
        }

        // https://stackoverflow.com/a/27376368
        public static string GetListenIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        public SocketServer()
        {
            var t = new Thread(Start);
            t.Start();
        }

        void Start()
        {
            Server = new WebSocketServer("ws://0.0.0.0:26401");
            Server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Connections.Add(socket);

                    // Send the handshake packet
                    var machineName = Encoding.UTF8.GetBytes(Environment.MachineName);
                    var machineNamePacket = new byte[machineName.Length + 1];
                    machineNamePacket[0] = 0xA; // Identifies the packet as the handshake packet
                    machineName.CopyTo(machineNamePacket, 1);
                    socket.Send(machineNamePacket);
                };
                socket.OnClose = () => Connections.Remove(socket);
                socket.OnError = (_) => Connections.Remove(socket);
                socket.OnMessage = message => Debug.WriteLine(message);
            });
        }
    }
}
