using IgniteView.Core;
using IgniteView.Desktop;
using Rssdp;

namespace Oberon.Remote.Desktop
{
    internal class Program
    {
        public static SocketServer SocketServer;
        public static InputModule InputModule;

        [STAThread]
        static void Main(string[] args)
        {
            SocketServer = new SocketServer();
            InputModule = new WGIInputModule();

            DesktopPlatformManager.Activate();
            var app = new ViteAppManager();

            var mainWindow =
                WebWindow.Create()
                .WithTitle("Oberon Remote")
                .WithBounds(new LockedWindowBounds(400, 640))
                .Show();

            app.Run();
        }
    }
}
