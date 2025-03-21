using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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
                return (App.Instance.Client?.Client?.State == System.Net.WebSockets.WebSocketState.Open) && App.Instance.Client.Remote.InternalID == InternalID;
            }
        }

        [JsonIgnore]
        public string ConnectedText
        {
            get
            {
                return IsConnected ? "✓ Connected" : "× Disconnected";
            }
        }

        [JsonIgnore]
        public Brush Background
        {
            get
            {
                return IsConnected ? new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]) : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }

        [JsonIgnore]
        public string DisplayAddress
        {
            get
            {
                var ip = IPAddress.Replace("ws://", "");
                ip = ip.Replace("wss://", "");
                ip = ip.Replace(":26401", "");
                ip = ip.Replace("/", "");
                return ip;
            }
        }

        [JsonIgnore]
        public string Nonce
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
