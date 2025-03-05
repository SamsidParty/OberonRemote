using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon.Remote.Desktop
{
    internal class CommanderClient
    {
        public IWebSocketConnection Connection;

        public CommanderClient(IWebSocketConnection conn)
        {
            Connection = conn;
        }
    }
}
