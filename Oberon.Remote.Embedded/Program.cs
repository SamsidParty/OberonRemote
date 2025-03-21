using Oberon.Remote.Core;

namespace Oberon.Remote.Embedded {
    public class Program {
        public static void Main(string[] args) {

            ServerStatus.CurrentStatus.MachineName = "[Embedded Remote] " + Environment.MachineName;
            OberonManager.Initialize(new LinuxInputModule());

            new Thread(() => (OberonManager.InputModule as LinuxInputModule).StartProcessingInputs()).Start();

            Console.WriteLine("Started Input Server");
            Console.WriteLine("Listening On IP Address: " + SocketServer.GetListenIP());
            Console.WriteLine("Press Any Key To Stop");
            
            Console.ReadKey();

            Console.WriteLine("Stopping...");
            OberonManager.SocketServer.Stop();
        }
    }
}