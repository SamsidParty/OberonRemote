﻿using IgniteView.Core;
using IgniteView.Desktop;
using Rssdp;

namespace Oberon.Remote.Desktop
{
    internal class Program
    {
        public static SocketServer SocketServer;

        [STAThread]
        static void Main(string[] args)
        {
            SocketServer = new SocketServer();

            DesktopPlatformManager.Activate();
            var app = new ViteAppManager();

            var mainWindow =
                WebWindow.Create()
                .WithTitle("Hello, world")
                .Show();

            app.Run();
        }
    }
}
