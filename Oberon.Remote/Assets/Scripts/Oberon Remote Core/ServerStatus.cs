using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon.Remote.Core
{
    public class ServerStatus
    {
        public string ListenIP => SocketServer.GetListenIP();
        public string InputModuleType => OberonManager.InputModule.GetType().Name;

        public static ServerStatus GetServerStatus()
        {
            return new ServerStatus();
        }
    }
}
