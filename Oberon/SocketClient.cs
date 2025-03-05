using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Oberon
{
    public class SocketClient
    {
        public WebSocket Client;
        public PairedRemote Remote;

        public SocketClient(PairedRemote remote)
        {
            Remote = remote;
            new Thread(() => Start(Remote.IPAddress)).Start();
        }

        void Start(string host)
        {
            InputForwarder.Instance.Reset();

            Client = new WebSocket(host);
            Client.Connect();
            Client.OnMessage += OnMessage;
            Client.OnClose += OnClose;
            Client.OnError += OnClose;
        }

        void OnClose(object sender, EventArgs e)
        {
            InputForwarder.Instance.Reset();
            App.Instance.Client = null;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var data = e.RawData;

            // 0xFF Represents a controller input packet
            if (data[0] == 0xFF)
            {
                InputForwarder.Instance.InjectInput(data);
            }
        }

        public void Close()
        {
            Client.Close();
        }
    }
}
