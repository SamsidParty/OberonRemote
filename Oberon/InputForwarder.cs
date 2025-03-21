using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.Gaming.UI;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Xaml.Controls;

namespace Oberon
{
    public class InputForwarder
    {
        public static RumbleState[] RumbleValues = new RumbleState[] { new RumbleState(), new RumbleState(), new RumbleState(), new RumbleState() };
        public InputInjector Injector;

        public InjectedInputGamepadInfo GamepadState;
        public bool XboxButtonPressed = false;

        public static void ResetAll()
        {
            for (int i = 0; i < App.Instance.InputInjectors.Length; i++)
            {
                if (App.Instance.InputInjectors[i] == null) continue;
                App.Instance.InputInjectors[i].Reset();
                App.Instance.InputInjectors[i] = null;
            }
        }

        public InputForwarder() {
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
        public void InjectInput(byte[] encoded, int offset)
        {
            // Axes and triggers
            GamepadState.LeftThumbstickX = (double)BitConverter.ToInt16(encoded, 1 + offset) / (double)Int16.MaxValue;
            GamepadState.LeftThumbstickY = (double)BitConverter.ToInt16(encoded, 3 + offset) / (double)-Int16.MaxValue;
            GamepadState.RightThumbstickX = (double)BitConverter.ToInt16(encoded, 5 + offset) / (double)Int16.MaxValue;
            GamepadState.RightThumbstickY = (double)BitConverter.ToInt16(encoded, 7 + offset) / (double)-Int16.MaxValue;
            GamepadState.LeftTrigger = (double)BitConverter.ToInt16(encoded, 9 + offset) / (double)Int16.MaxValue;
            GamepadState.RightTrigger = (double)BitConverter.ToInt16(encoded, 11 + offset) / (double)Int16.MaxValue;

            // Buttons
            GamepadState.Buttons = GamepadButtons.None;
            var menuComboPressed = (((encoded[13 + offset] & (1 << 1)) != 0) && ((encoded[13 + offset] & (1 << 0)) != 0));  // Checks if menu and view are pressed at the same time

            // Button group 1
            GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 7)) != 0) ? GamepadButtons.A : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 6)) != 0) ? GamepadButtons.B : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 5)) != 0) ? GamepadButtons.X : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 4)) != 0) ? GamepadButtons.Y : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 3)) != 0) ? GamepadButtons.LeftShoulder : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 2)) != 0) ? GamepadButtons.RightShoulder : GamepadButtons.None;

            if (!menuComboPressed)
            {
                GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 1)) != 0) ? GamepadButtons.Menu : GamepadButtons.None;
                GamepadState.Buttons |= ((encoded[13 + offset] & (1 << 0)) != 0) ? GamepadButtons.View : GamepadButtons.None;
            }


            // Button group 2
            GamepadState.Buttons |= ((encoded[14 + offset] & (1 << 6)) != 0) ? GamepadButtons.LeftThumbstick : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14 + offset] & (1 << 5)) != 0) ? GamepadButtons.RightThumbstick : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14 + offset] & (1 << 4)) != 0) ? GamepadButtons.DPadUp : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14 + offset] & (1 << 3)) != 0) ? GamepadButtons.DPadDown : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14 + offset] & (1 << 2)) != 0) ? GamepadButtons.DPadLeft : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14 + offset] & (1 << 1)) != 0) ? GamepadButtons.DPadRight : GamepadButtons.None;

            // Xbox Button
            if (menuComboPressed || ((encoded[14 + offset] & (1 << 0)) != 0))
            { 
                if (!XboxButtonPressed)
                {
                    XboxButtonPressed = true;
                    App.Instance.Persistence.PlayRemoteSound();
                    Injector.InjectShortcut(InjectedInputShortcut.Start);

                    // Vibrate
                    foreach (var r in RumbleValues) { r.SetAll(255); }

                    // Reset vibrations
                    Task.Run(async () =>
                    {
                        await Task.Delay(50);
                        foreach (var r in RumbleValues) { r.SetAll(0); }
                    });
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
            Injector.UninitializeGamepadInjection();
        }
    }
}
