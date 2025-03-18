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
                byte[] requestPacket = new byte[] { 0xFA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                while (Client.State == WebSocketState.Open)
                {
                    for (int i = 0; i < 4; i++) {
                        requestPacket[(i * 4) + 1] = InputForwarder.RumbleValues[i].LeftMotor;
                        requestPacket[(i * 4) + 2] = InputForwarder.RumbleValues[i].RightMotor;
                        requestPacket[(i * 4) + 3] = InputForwarder.RumbleValues[i].LeftTrigger;
                        requestPacket[(i * 4) + 4] = InputForwarder.RumbleValues[i].RightTrigger;
                    }

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
            MainPage.Instance.RefreshSettings();
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

                if (data[25] == 0xFF && App.Instance.InputInjectors[1] == null)
                {
                    App.Instance.InputInjectors[1] = new InputForwarder(); // Init controller 2
                }
                else if (data[25] != 0xFF && App.Instance.InputInjectors[1] != null)
                {
                    App.Instance.InputInjectors[1].Reset(); // Delete controller 2
                    App.Instance.InputInjectors[1] = null;
                }

                if (data[50] == 0xFF && App.Instance.InputInjectors[2] == null)
                {
                    App.Instance.InputInjectors[2] = new InputForwarder(); // Init controller 3
                }
                else if (data[50] != 0xFF && App.Instance.InputInjectors[2] != null)
                {
                    App.Instance.InputInjectors[2].Reset(); // Delete controller 3
                    App.Instance.InputInjectors[2] = null;
                }

                if (data[75] == 0xFF && App.Instance.InputInjectors[3] == null)
                {
                    App.Instance.InputInjectors[3] = new InputForwarder(); // Init controller 4
                }
                else if (data[75] != 0xFF && App.Instance.InputInjectors[3] != null)
                {
                    App.Instance.InputInjectors[3].Reset(); // Delete controller 4
                    App.Instance.InputInjectors[3] = null;
                }

                for (int i = 0; i < App.Instance.InputInjectors.Length; i++)
                {
                    if (App.Instance.InputInjectors[i] == null) continue;
                    App.Instance.InputInjectors[i].InjectInput(data, i * 25);
                }
            }
        }

        public void Close()
        {
            Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
    }
}
