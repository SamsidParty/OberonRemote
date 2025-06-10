using Oberon.Remote.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityInputModule : InputModule
{

    void InsertInputBytes(byte[] bytes, int offset, Gamepad gamepad, Keyboard keyboard)
    {
        bytes[offset] = 0xFF;
        InsertUInt16(bytes, 1 + offset, Mathf.FloorToInt((gamepad?.leftStick.x.value ?? 0) * 32767));
        InsertUInt16(bytes, 3 + offset, Mathf.FloorToInt((gamepad?.leftStick.y.value ?? 0) * -32767));
        InsertUInt16(bytes, 5 + offset, Mathf.FloorToInt((gamepad?.rightStick.x.value ?? 0) * 32767));
        InsertUInt16(bytes, 7 + offset, Mathf.FloorToInt((gamepad?.rightStick.y.value ?? 0) * -32767));
        InsertUInt16(bytes, 9 + offset, Mathf.FloorToInt((gamepad?.leftTrigger.value ?? 0) * 32767));
        InsertUInt16(bytes, 11 + offset, Mathf.FloorToInt((gamepad?.rightTrigger.value ?? 0) * 32767));

        var buttonGroup1 = new bool?[] { 
            gamepad?.buttonSouth.isPressed | keyboard?.enterKey.isPressed,
            gamepad?.buttonEast.isPressed | keyboard?.escapeKey.isPressed,
            gamepad?.buttonWest.isPressed,
            gamepad?.buttonNorth.isPressed,
            gamepad?.leftShoulder.isPressed,
            gamepad?.rightShoulder.isPressed,
            gamepad?.startButton.isPressed,
            gamepad?.selectButton.isPressed
        };
        var buttonGroup2 = new bool?[] { 
            false,
            gamepad?.leftStickButton.isPressed,
            gamepad?.rightStickButton.isPressed,
            gamepad?.dpad.up.isPressed | keyboard?.upArrowKey.isPressed,
            gamepad?.dpad.down.isPressed | keyboard?.downArrowKey.isPressed,
            gamepad?.dpad.left.isPressed | keyboard?.leftArrowKey.isPressed,
            gamepad?.dpad.right.isPressed | keyboard?.rightArrowKey.isPressed,
            keyboard?.ctrlKey.isPressed
        };

        bytes[13 + offset] = GenerateButtonGroup(buttonGroup1);
        bytes[14 + offset] = GenerateButtonGroup(buttonGroup2);
    }


    public void ProcessInput()
    {
        var bytes = CurrentControllerState;

        for (int i = 0; i < 4; i++)
        {
            // Try to get the gamepad at index i or null
            var gamepad = (i >= Gamepad.all.Count) ? null : Gamepad.all[i];

            gamepad?.SetMotorSpeeds(RumbleValues[i].LeftMotor / 255f, RumbleValues[i].RightMotor / 255f);
            InsertInputBytes(bytes, i * 25, gamepad, i == 0 ? Keyboard.current : null);
        }

        bytes[0] = 0xFF;
        CurrentControllerState = bytes;
        UpdateStatus();
    }

    void UpdateStatus()
    {
        ServerStatus.CurrentStatus.ConnectedControllerCount = Gamepad.all.Count;
    }

    #region Helpers

    public static byte GenerateButtonGroup(bool?[] source)
    {
        byte result = 0;
        int index = 8 - source.Length;

        foreach (bool? b in source)
        {
            if (b ?? false)
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
