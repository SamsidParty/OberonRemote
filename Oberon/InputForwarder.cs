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
        public InputInjector Injector;

        public InjectedInputGamepadInfo GamepadState;
        public bool XboxButtonPressed = false;
        public Gamepad AssociatedGamepad;
        public int GamepadIndex = 1;

        public byte[] VibrationBytes
        {
            get
            {
                if (AssociatedGamepad == null) return new byte[4];

                byte[] vibration = new byte[4];

                vibration[0] = (byte)Math.Ceiling(AssociatedGamepad.Vibration.LeftMotor * 255f);
                vibration[1] = (byte)Math.Ceiling(AssociatedGamepad.Vibration.RightMotor * 255f);
                vibration[2] = (byte)Math.Ceiling(AssociatedGamepad.Vibration.LeftTrigger * 255f);
                vibration[3] = (byte)Math.Ceiling(AssociatedGamepad.Vibration.RightTrigger * 255f);

                return vibration;
            }
        }

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

            Gamepad.GamepadAdded += (object sender, Gamepad e) =>
            {
                // Set a specific magic input that a human can't possible replicate
                var detectionState = new InjectedInputGamepadInfo();
                detectionState.LeftTrigger = 0.7;
                detectionState.RightTrigger = 0.2;
                Injector.InjectGamepadInput(detectionState);

                // Detect if the gamepad is at that specific value
                var reading = e.GetCurrentReading();
                var leftTriggerReading = reading.LeftTrigger;
                var rightTriggerReading = reading.RightTrigger;

                if (leftTriggerReading > 0.68 && leftTriggerReading < 0.72 && rightTriggerReading > 0.18 && rightTriggerReading < 0.22)
                {
                    AssociatedGamepad = e;
                }

                Update(); // Reset internal gamepad state
            };


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
            var menuComboPressed = (((encoded[13] & (1 << 1)) != 0) && ((encoded[13] & (1 << 0)) != 0));  // Checks if menu and view are pressed at the same time

            // Button group 1
            GamepadState.Buttons |= ((encoded[13] & (1 << 7)) != 0) ? GamepadButtons.A : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 6)) != 0) ? GamepadButtons.B : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 5)) != 0) ? GamepadButtons.X : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 4)) != 0) ? GamepadButtons.Y : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 3)) != 0) ? GamepadButtons.LeftShoulder : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[13] & (1 << 2)) != 0) ? GamepadButtons.RightShoulder : GamepadButtons.None;

            if (!menuComboPressed)
            {
                GamepadState.Buttons |= ((encoded[13] & (1 << 1)) != 0) ? GamepadButtons.Menu : GamepadButtons.None;
                GamepadState.Buttons |= ((encoded[13] & (1 << 0)) != 0) ? GamepadButtons.View : GamepadButtons.None;
            }


            // Button group 2
            GamepadState.Buttons |= ((encoded[14] & (1 << 6)) != 0) ? GamepadButtons.LeftThumbstick : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 5)) != 0) ? GamepadButtons.RightThumbstick : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 4)) != 0) ? GamepadButtons.DPadUp : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 3)) != 0) ? GamepadButtons.DPadDown : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 2)) != 0) ? GamepadButtons.DPadLeft : GamepadButtons.None;
            GamepadState.Buttons |= ((encoded[14] & (1 << 1)) != 0) ? GamepadButtons.DPadRight : GamepadButtons.None;

            // Xbox Button
            if (menuComboPressed || ((encoded[14] & (1 << 0)) != 0))
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
            if (AssociatedGamepad == null) { return; }

            Debug.WriteLine(VibrationBytes[0] + " " + VibrationBytes[1] + " " + VibrationBytes[2] + " " + VibrationBytes[3]);

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
