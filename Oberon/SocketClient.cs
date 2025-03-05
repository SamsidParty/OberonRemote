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
        public WebSocket Server;

        public SocketClient(string host)
        {
            new Thread(() => Start(host)).Start();
        }

        void Start(string host)
        {
            Server = new WebSocket("ws://" + host);
            Server.Connect();
            Server.OnMessage += OnMessage;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var data = e.RawData;

            // 0xFF Represents an input packet
            if (data[0] == 0xFF)
            {
                InputForwarder.Instance.InjectInput(data);
            }
        }
    }
}
