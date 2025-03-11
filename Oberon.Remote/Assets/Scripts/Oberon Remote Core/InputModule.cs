using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon.Remote.Core
{
    public class InputModule
    {
        public byte[] CurrentControllerState = new byte[20];

        public InputModule()
        {
            CurrentControllerState[0] = 0xFF;
        }
    }
}
