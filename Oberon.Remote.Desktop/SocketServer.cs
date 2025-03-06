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

        SynchronizedCollection<CommanderClient> Connections = new SynchronizedCollection<CommanderClient>();
        static byte[] CurrentControllerState = new byte[20];

        [Command("forwardInput")]
        public static void ForwardInput(string input)
        {
            CurrentControllerState = Convert.FromBase64String(input);
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
            CurrentControllerState[0] = 0xFF;

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
                    var client = new CommanderClient(socket);
                    Connections.Add(client);

                    socket.OnClose = () => Connections.Remove(client);
                    socket.OnBinary = (data) =>
                    {
                        if (data[0] == 0xFA) // Request the current controller packet
                        {
                            socket.Send(CurrentControllerState); // Send the latest controller packet
                        }
                    };

                    // Send the handshake packet
                    var machineName = Encoding.UTF8.GetBytes("Cloud Remote");
                    var machineNamePacket = new byte[machineName.Length + 1];
                    machineNamePacket[0] = 0xA; // Identifies the packet as the handshake packet
                    machineName.CopyTo(machineNamePacket, 1);
                    socket.Send(machineNamePacket);
                };
                socket.OnError = (_) => { };
            });
        }
    }
}
