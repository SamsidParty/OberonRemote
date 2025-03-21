using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon
{
    public class Networking
    {
        public static string FormatWebSocketHost(string host)
        {
            var newHost = host.ToLower(); 

            // Protocol
            if (!newHost.StartsWith("ws://") || newHost.StartsWith("wss://"))
            {
                newHost = "ws://" + newHost;
            }

            // Remove trailing slash (will be added back later)
            if (newHost.EndsWith("/"))
            {
                newHost = newHost.Substring(0, newHost.Length - 1);
            }

            // Port
            if (newHost.Split(':').Count() < 3) {
                newHost += ":26401"; // Default port for the remote
            }

            return newHost + "/";
        }
    }
}
