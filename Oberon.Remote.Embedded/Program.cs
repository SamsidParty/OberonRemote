using Microsoft.Xna.Framework;
using Oberon.Remote.Core;

namespace Oberon.Remote.Embedded {
    public class Program {
        public static void Main(string[] args) {

            ServerStatus.CurrentStatus.MachineName = "[Embedded Remote] " + Environment.MachineName;
            OberonManager.Initialize(new EmbeddedInputModule());


            Console.WriteLine("Started Input Server");
            Console.WriteLine("Listening On IP Address: " + SocketServer.GetListenIP());
            
            var game = new HeadlessGame();
            game.Run();

            OberonManager.SocketServer.Stop();
        }
    }
}