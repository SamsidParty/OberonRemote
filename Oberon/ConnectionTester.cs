using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Oberon
{
    public class ConnectionTester
    {
        public static async Task<string> TestConnectionToIP(string ip)
        {
            try
            {
                using (var ws = new ClientWebSocket())
                {
                    await ws.ConnectAsync(new Uri(Networking.FormatWebSocketHost(ip)), CancellationToken.None);
                    byte[] buffer = new byte[256];

                    while (ws.State == WebSocketState.Open)
                    {
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary && buffer[0] == 0xA) // first byte = 0xA identifies the packet as the handshake packet
                        {
                            return "Success" + Encoding.UTF8.GetString(buffer).Split("\0")[0];
                        }
                    }

                    throw new Exception("Failed to obtain handshake packet from remote");
                }
            }
            catch (Exception ex) {
                return "Failed to connect to the remote: " + ex.Message;
            }
        }
    }
}
