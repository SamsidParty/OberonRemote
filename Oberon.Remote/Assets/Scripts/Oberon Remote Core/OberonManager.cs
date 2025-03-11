using Oberon.Remote.Core;

namespace Oberon.Remote.Core
{
    public class OberonManager
    {
        public static InputModule InputModule;
        public static SocketServer SocketServer;

        public static void Initialize(InputModule module)
        {
            InputModule = module;
            SocketServer = new SocketServer();
        }
    }
}
