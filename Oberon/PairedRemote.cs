using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon
{
    public class PairedRemote
    {
        public string IPAddress;
        public string DisplayName;
        public string InternalID;

        [JsonIgnore]
        public bool IsConnected
        {
            get
            {
                return (App.Instance.Client?.Client?.IsAlive ?? false) && App.Instance.Client.Host == IPAddress;
            }
        }

        [JsonIgnore]
        public string ConnectedText
        {
            get
            {
                return IsConnected ? "Connected" : "Disconnected";
            }
        }
    }
}
