using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            InputForwarder.ResetAll();
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
                        throw new Exception("Socket closed");
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
                // Disconnect notification
                new ToastContentBuilder()
                    .AddText("Remote Disconnected")
                    .AddText(Remote.DisplayName)
                    .Show();

                OnClose(null, null);
            }
        }

        void OnClose(object sender, EventArgs e)
        {
            InputForwarder.ResetAll();
            App.Instance.Client = null;
            GC.Collect();

            if (MainPage.Instance != null)
            {
                MainPage.Instance.RefreshSettings();
            }
        }

        private void OnMessage(byte[] data)
        {
            // Make sure only one client exists at a time
            if (App.Instance.Client != this)
            {
                Close();
                return;
            }

            
            if (data[0] == 0xFF) // 0xFF Represents a controller input packet
            {
                if (App.Instance.InputInjectors[0] == null)
                {
                    App.Instance.InputInjectors[0] = new InputForwarder(); // Init controller 1
                }

                for (int i = 0; i < App.Instance.InputInjectors.Length; i++)
                {
                    if (App.Instance.InputInjectors[i] == null) continue;
                    App.Instance.InputInjectors[i].InjectInput(data);
                }
            }
        }

        public void Close()
        {
            Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
    }
}
