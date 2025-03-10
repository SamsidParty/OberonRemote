using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon.Remote.Desktop
{
    public class ServerStatus
    {
        public string ListenIP => SocketServer.GetListenIP();

        [Command("getServerStatus")]
        public static ServerStatus GetServerStatus()
        {
            return new ServerStatus();
        }
    }
}
