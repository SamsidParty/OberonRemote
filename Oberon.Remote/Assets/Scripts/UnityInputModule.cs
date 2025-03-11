using Oberon.Remote.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityInputModule : InputModule
{

    void InsertInputBytes(byte[] bytes, int offset, Gamepad gamepad)
    {
        bytes[offset] = 0xFF;
        InsertUInt16(bytes, 1 + offset, Mathf.FloorToInt(gamepad.leftStick.x.value * 32767));
        InsertUInt16(bytes, 3 + offset, Mathf.FloorToInt(gamepad.leftStick.y.value * -32767));
        InsertUInt16(bytes, 5 + offset, Mathf.FloorToInt(gamepad.rightStick.x.value * 32767));
        InsertUInt16(bytes, 7 + offset, Mathf.FloorToInt(gamepad.rightStick.y.value * -32767));
        InsertUInt16(bytes, 9 + offset, Mathf.FloorToInt(gamepad.leftTrigger.value * 32767));
        InsertUInt16(bytes, 11 + offset, Mathf.FloorToInt(gamepad.rightTrigger.value * 32767));

        var buttonGroup1 = new bool[] { gamepad.buttonSouth.isPressed, gamepad.buttonEast.isPressed, gamepad.buttonWest.isPressed, gamepad.buttonNorth.isPressed, gamepad.leftShoulder.isPressed, gamepad.rightShoulder.isPressed, gamepad.startButton.isPressed, gamepad.selectButton.isPressed };
        var buttonGroup2 = new bool[] { false, gamepad.leftStickButton.isPressed, gamepad.rightStickButton.isPressed, gamepad.dpad.up.isPressed, gamepad.dpad.down.isPressed, gamepad.dpad.left.isPressed, gamepad.dpad.right.isPressed, false };

        bytes[13 + offset] = GenerateButtonGroup(buttonGroup1);
        bytes[14 + offset] = GenerateButtonGroup(buttonGroup2);
    }

    void InsertNullInputBytes(byte[] bytes, int offset)
    {
        bytes[offset] = 0xFF;
        InsertUInt16(bytes, 1 + offset, 0);
        InsertUInt16(bytes, 3 + offset, 0);
        InsertUInt16(bytes, 5 + offset, 0);
        InsertUInt16(bytes, 7 + offset, 0);
        InsertUInt16(bytes, 9 + offset, 0);
        InsertUInt16(bytes, 11 + offset, 0);

        bytes[13 + offset] = 0;
        bytes[14 + offset] = 0;
    }

    public void ProcessInput()
    {
        var gamepad = Gamepad.current;
        var bytes = CurrentControllerState;

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (i >= 4) // Max 4 Gamepads
            {
                break;
            }

            if (Gamepad.all[i] == null)
            {
                InsertNullInputBytes(bytes, i * 25);
                continue;
            }
            InsertInputBytes(bytes, i * 25, Gamepad.all[i]);
        }

        UpdateStatus();
    }

    void UpdateStatus()
    {
        ServerStatus.CurrentStatus.ConnectedControllerCount = Gamepad.all.Count;
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
