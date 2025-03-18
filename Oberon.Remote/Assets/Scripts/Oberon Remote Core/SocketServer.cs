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
using System.Collections.Concurrent;

namespace Oberon.Remote.Core
{
    public class SocketServer
    {
        Thread ServerThread;
        WebSocketServer Server;

        SynchronizedCollection<CommanderClient> Connections = new SynchronizedCollection<CommanderClient>();

        RumbleState[] RumbleState = new RumbleState[4];

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
            ServerThread = new Thread(Start);
            ServerThread.Start();
        }

        void Start()
        {
            Server = new WebSocketServer("ws://0.0.0.0:26401");
            Server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    ResetRumble();

                    var client = new CommanderClient(socket);
                    Connections.Add(client);

                    socket.OnClose = () => { Connections.Remove(client); ResetRumble(); };
                    socket.OnBinary = (data) =>
                    {
                        if (Server == null)
                        {
                            socket.Close();
                            return;
                        }

                        if (data[0] == 0xFA) // Request the current controller packet
                        {
                            if (data.Length == 17) // The packet contains rumble information
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    RumbleState[i].LeftMotor = data[(i * 4) + 1];
                                    RumbleState[i].RightMotor = data[(i * 4) + 2];
                                    RumbleState[i].LeftTrigger = data[(i * 4) + 3];
                                    RumbleState[i].RightTrigger = data[(i * 4) + 4];
                                }
                            }

                            UpdateRumble();
                            socket.Send(OberonManager.InputModule.CurrentControllerState); // Send the latest controller packet
                        }
                    };

                    // Send the handshake packet
                    var machineName = Encoding.UTF8.GetBytes(ServerStatus.CurrentStatus.MachineName);
                    var machineNamePacket = new byte[machineName.Length + 1];
                    machineNamePacket[0] = 0xA; // Identifies the packet as the handshake packet
                    machineName.CopyTo(machineNamePacket, 1);
                    socket.Send(machineNamePacket);
                };
                socket.OnError = (_) => { ResetRumble(); };
            });
        }

        void UpdateRumble()
        {
            OberonManager.InputModule.RumbleValues = RumbleState;
        }

        void ResetRumble()
        {
            RumbleState = new RumbleState[4];
            UpdateRumble();
        }

        public void Stop()
        {
            if (ServerThread != null)
            {
                ResetRumble();
                Server.ListenerSocket.Close();
                Server.Dispose();
                ServerThread.Abort();
                ServerThread = null;
                Server = null;
            }
        }
    }
}
