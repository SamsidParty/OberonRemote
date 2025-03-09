using System.Net.Sockets;
using System.Net;

namespace Oberon.Remote.MAUI;

public partial class ServerStatus : ContentView
{
    // https://stackoverflow.com/a/27376368
    public static string ListenIP
    {
        get
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
    }

    public static string DisplayedIP => "IP Address: " + ListenIP;

    public ServerStatus()
	{
		InitializeComponent();
	}
}