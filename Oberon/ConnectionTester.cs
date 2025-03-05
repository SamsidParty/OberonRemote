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
                CancellationTokenSource source = new CancellationTokenSource();
                using (var ws = new ClientWebSocket())
                {
                    await ws.ConnectAsync(new Uri(Networking.FormatWebSocketHost(ip)), CancellationToken.None);
                    byte[] buffer = new byte[256];

                    while (ws.State == WebSocketState.Open)
                    {
                        return "Success";

                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        }
                        else
                        {
                            
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
