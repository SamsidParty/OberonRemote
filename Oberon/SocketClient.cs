using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oberon
{
    public class SocketClient
    {
        public ClientWebSocket Client;
        public PairedRemote Remote;

        public SocketClient(PairedRemote remote)
        {
            Remote = remote;
            new Thread(() => Start(Remote.IPAddress)).Start();
        }

        async void Start(string host)
        {
            InputForwarder.Instance.Reset();
            Client = new ClientWebSocket();

            try
            {
                await Client.ConnectAsync(new Uri(host), CancellationToken.None);

                byte[] buffer = new byte[256];
                byte[] requestPacket = new byte[] { 0xFA };

                while (Client.State == WebSocketState.Open)
                {
                    await Client.SendAsync(requestPacket, WebSocketMessageType.Binary, true, CancellationToken.None);
                    var result = await Client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        OnMessage(buffer);   
                    }
                }

                throw new Exception("Connection closed");
            }
            catch
            {
                OnClose(null, null);
            }
        }

        void OnClose(object sender, EventArgs e)
        {
            InputForwarder.Instance.Reset();
            App.Instance.Client = null;
            GC.Collect();
        }

        private void OnMessage(byte[] data)
        {
            // Make sure only one client exists at a time
            if (App.Instance.Client != this)
            {
                Close();
                return;
            }

            // 0xFF Represents a controller input packet
            if (data[0] == 0xFF)
            {
                InputForwarder.Instance.InjectInput(data);
            }
        }

        public void Close()
        {
            Client.Dispose();
        }
    }
}
