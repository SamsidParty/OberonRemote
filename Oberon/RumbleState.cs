using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oberon
{
    public class RumbleState
    {
        public byte LeftMotor;
        public byte RightMotor;
        public byte LeftTrigger;
        public byte RightTrigger;

        public void SetAll(byte val)
        {
            LeftMotor = val;
            RightMotor = val;
            LeftTrigger = val;
            RightTrigger = val;
        }
    }
}
