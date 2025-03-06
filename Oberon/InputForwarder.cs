using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Xaml.Controls;

namespace Oberon
{
    public class InputForwarder
    {
        public static InputForwarder Instance;
        public InputInjector Injector;


        public InjectedInputGamepadInfo GamepadState;
        public bool XboxButtonPressed = false;

        public InputForwarder() { 
            Instance = this;
            Injector = InputInjector.TryCreate();
            Injector.InitializeGamepadInjection();
            GamepadState = new InjectedInputGamepadInfo();
        }

        // Injected input format
        // [0] = 0xFF = Magic byte, identifies that the packet should inject input
        // [1, 2] = Int16 = Left thumbstick X
        // [3, 4] = Int16 = Left thumbstick Y
        // [5, 6] = Int16 = Right thumbstick X
        // [7, 8] = Int16 = Right thumbstick Y
        // [9, 10] = Int16 = Left trigger
        // [11, 12] = Int16 = Right trigger
        // [13] = Bitmask = A, B, X, Y, LB, RB, Menu, View
        public void InjectInput(byte[] encoded)
        {
            // Axes and triggers
            GamepadState.LeftThumbstickX = (double)BitConverter.ToInt16(encoded, 1) / (double)Int16.MaxValue;
            GamepadState.LeftThumbstickY = (double)BitConverter.ToInt16(encoded, 3) / (double)-Int16.MaxValue;
            GamepadState.RightThumbstickX = (double)BitConverter.ToInt16(encoded, 5) / (double)Int16.MaxValue;
            GamepadState.RightThumbstickY = (double)BitConverter.ToInt16(encoded, 7) / (double)-Int16.MaxValue;
            GamepadState.LeftTrigger = (double)BitConverter.ToInt16(encoded, 9) / (double)Int16.MaxValue;
            GamepadState.RightTrigger = (double)BitConverter.ToInt16(encoded, 11) / (double)Int16.MaxValue;

            // Buttons
            GamepadState.Buttons = GamepadButtons.None;

            // Button group 1
            GamepadState.Buttons |= ((encoded[13] & (1 << 7)) != 0) ? GamepadButtons.A : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 6)) != 0) ? GamepadButtons.B : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 5)) != 0) ? GamepadButtons.X : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 4)) != 0) ? GamepadButtons.Y : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 3)) != 0) ? GamepadButtons.LeftShoulder : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 2)) != 0) ? GamepadButtons.RightShoulder : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 1)) != 0) ? GamepadButtons.Menu : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 0)) != 0) ? GamepadButtons.View : GamepadButtons.None;

            // Button group 2
            GamepadState.Buttons |= ((encoded[14] & (1 << 6)) != 0) ? GamepadButtons.LeftThumbstick : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 5)) != 0) ? GamepadButtons.RightThumbstick : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 4)) != 0) ? GamepadButtons.DPadUp : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 3)) != 0) ? GamepadButtons.DPadDown : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 2)) != 0) ? GamepadButtons.DPadLeft : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 1)) != 0) ? GamepadButtons.DPadRight : GamepadButtons.None;

            // Xbox Button
            if ((((encoded[13] & (1 << 1)) != 0) && ((encoded[13] & (1 << 0)) != 0)) || ((encoded[14] & (1 << 0)) != 0)) // Checks if menu and view are pressed at the same time
            { 
                if (!XboxButtonPressed)
                {
                    XboxButtonPressed = true;
                    App.Instance.Persistence.PlayRemoteSound();
                    Injector.InjectShortcut(InjectedInputShortcut.Start);
                }
            }
            else
            {
                XboxButtonPressed = false;
            }


            Update();
        }


        public void Update()
        {
            Injector.InjectGamepadInput(GamepadState);
        }

        public void Reset()
        {
            XboxButtonPressed = false;
            GamepadState = new InjectedInputGamepadInfo();
            Update();
        }
    }
}
