using IgniteView.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon.Remote.Desktop
{
    public class JSInputModule : InputModule
    {
        [Command("forwardInput")]
        public static void ForwardInput(string input)
        {
            Program.InputModule.CurrentControllerState = Convert.FromBase64String(input);
        }
    }
}
