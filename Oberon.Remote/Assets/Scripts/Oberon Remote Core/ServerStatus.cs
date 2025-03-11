using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon.Remote.Core
{
    public class ServerStatus
    {
        public static ServerStatus CurrentStatus = new ServerStatus();

        public string ListenIP => SocketServer.GetListenIP();
        public string InputModuleType => OberonManager.InputModule.GetType().Name;

        public string MachineName;

        public int ConnectedControllerCount;
    }
}
