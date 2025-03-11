using Oberon.Remote.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityInputModule : InputModule
{
    public void ProcessInput()
    {
        var gamepad = Gamepad.current;
        var bytes = CurrentControllerState;

        InsertUInt16(bytes, 1, Mathf.FloorToInt(gamepad.leftStick.x.value * 32767));
        InsertUInt16(bytes, 3, Mathf.FloorToInt(gamepad.leftStick.y.value * -32767));
        InsertUInt16(bytes, 5, Mathf.FloorToInt(gamepad.rightStick.x.value * 32767));
        InsertUInt16(bytes, 7, Mathf.FloorToInt(gamepad.rightStick.y.value * -32767));
        InsertUInt16(bytes, 9, Mathf.FloorToInt(gamepad.leftTrigger.value * 32767));
        InsertUInt16(bytes, 11, Mathf.FloorToInt(gamepad.rightTrigger.value * 32767));

        var buttonGroup1 = new bool[] { gamepad.buttonSouth.isPressed, gamepad.buttonEast.isPressed, gamepad.buttonWest.isPressed, gamepad.buttonNorth.isPressed, gamepad.leftShoulder.isPressed, gamepad.rightShoulder.isPressed, gamepad.startButton.isPressed, gamepad.selectButton.isPressed };
        var buttonGroup2 = new bool[] { false, gamepad.leftStickButton.isPressed, gamepad.rightStickButton.isPressed, gamepad.dpad.up.isPressed, gamepad.dpad.down.isPressed, gamepad.dpad.left.isPressed, gamepad.dpad.right.isPressed, false };

        bytes[13] = GenerateButtonGroup(buttonGroup1);
        bytes[14] = GenerateButtonGroup(buttonGroup2);
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
