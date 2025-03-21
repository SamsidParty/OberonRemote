using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Oberon.Remote.Core;

namespace Oberon.Remote.Embedded
{
    public class LinuxInputModule : InputModule
    {


        void InsertInputBytes(byte[] bytes, int offset, GamePadState gamepad)
        {
            bytes[offset] = 0xFF;
            InsertUInt16(bytes, 1 + offset, (int)Math.Floor(gamepad.ThumbSticks.Left.X * 32767));
            InsertUInt16(bytes, 3 + offset, (int)Math.Floor(gamepad.ThumbSticks.Left.Y * -32767));
            InsertUInt16(bytes, 5 + offset, (int)Math.Floor(gamepad.ThumbSticks.Right.X * 32767));
            InsertUInt16(bytes, 7 + offset, (int)Math.Floor(gamepad.ThumbSticks.Right.Y * -32767));
            InsertUInt16(bytes, 9 + offset, (int)Math.Floor(gamepad.Triggers.Left * 32767));
            InsertUInt16(bytes, 11 + offset, (int)Math.Floor(gamepad.Triggers.Right * 32767));

            var buttonGroup1 = new bool[] { gamepad.Buttons.A == ButtonState.Pressed, gamepad.Buttons.B == ButtonState.Pressed, gamepad.Buttons.X == ButtonState.Pressed, gamepad.Buttons.Y == ButtonState.Pressed, gamepad.Buttons.LeftShoulder == ButtonState.Pressed, gamepad.Buttons.RightShoulder == ButtonState.Pressed, gamepad.Buttons.Start == ButtonState.Pressed, gamepad.Buttons.Back == ButtonState.Pressed };
            var buttonGroup2 = new bool[] { false, gamepad.Buttons.LeftStick == ButtonState.Pressed, gamepad.Buttons.RightStick == ButtonState.Pressed, gamepad.DPad.Up == ButtonState.Pressed, gamepad.DPad.Down == ButtonState.Pressed, gamepad.DPad.Left == ButtonState.Pressed, gamepad.DPad.Right == ButtonState.Pressed, gamepad.Buttons.BigButton == ButtonState.Pressed };

            bytes[13 + offset] = GenerateButtonGroup(buttonGroup1);
            bytes[14 + offset] = GenerateButtonGroup(buttonGroup2);
        }

        void InsertNullInputBytes(byte[] bytes, int offset)
        {
            for (int i = 0; i < 24; i++)
            {
                bytes[offset + i] = 0;
            }
        }

        public override void ProcessInputs()
        {
            var bytes = CurrentControllerState;

            for (int i = 0; i < 4; i++)
            {
                var state = GamePad.GetState(i);
                if (!state.IsConnected)
                {
                    InsertNullInputBytes(bytes, i * 25);
                    continue;
                }

                GamePad.SetVibration(i, RumbleValues[i].LeftMotor / 255f, RumbleValues[i].RightMotor / 255f);
                InsertInputBytes(bytes, i * 25, state);
            }

            bytes[0] = 0xFF;
            CurrentControllerState = bytes;
            UpdateStatus();
        }

        void UpdateStatus()
        {
            ServerStatus.CurrentStatus.ConnectedControllerCount = GamePad.MaximumGamePadCount;
        }

        #region Helpers

        public static byte GenerateButtonGroup(bool[] source)
        {
            byte result = 0;
            int index = 8 - source.Length;

            foreach (bool b in source)
            {
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        public static void InsertUInt16(byte[] arr, int index, int value)
        {
            var bVal = BitConverter.GetBytes((UInt16)value);
            arr[index] = bVal[0];
            arr[index + 1] = bVal[1];
        } 

        #endregion
    }
}
