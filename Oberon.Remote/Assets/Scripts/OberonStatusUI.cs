using Oberon.Remote.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OberonStatusUI : MonoBehaviour
{
    public Text IPText;
    public Text ControllerText;
    public Text StickDebugText;

    public ControllerPreview[] ControllerTextures;
    public Image ControllerImage;

    string CurrentControllerImage;
    string LastControllerImage = "Missing";

    public GameObject ScreenSaverPanel;
    public bool IsScreenSaverEnabled;

    private void FixedUpdate()
    {
        var status = ServerStatus.CurrentStatus;
        IPText.text = "IP Address: " + status.ListenIP;

        if (status.ConnectedControllerCount == 0)
        {
            ControllerText.text = "Controller: Disconnected";
            CurrentControllerImage = "Missing";
        }
        else if (status.ConnectedControllerCount == 1)
        {
            ControllerText.text = "Controller: Connected";

            CurrentControllerImage = "Generic";

            if (Gamepad.current?.displayName.ToLower().Contains("dualsense") ?? false)
            {
                CurrentControllerImage = "PS5";
            }
        }
        else
        {
            ControllerText.text = "Controllers: " + status.ConnectedControllerCount + " Connected";
            CurrentControllerImage = "Generic";
        }

        if (CurrentControllerImage != LastControllerImage)
        {
            LastControllerImage = CurrentControllerImage;
            ControllerImage.sprite = ControllerTextures.Where((t) => t.ControllerType == CurrentControllerImage).First().Texture;
        }

        if (StickDebugText.gameObject.activeInHierarchy && Gamepad.current != null)
        {
            StickDebugText.text = "" + Gamepad.current.leftStick.value.ToString() + ", " + Gamepad.current.rightStick.value.ToString();
        }

        ScreenSaverPanel.SetActive(IsScreenSaverEnabled);
    }

    public void EnterScreenSaver() => IsScreenSaverEnabled = true;
    public void ExitScreenSaver() => IsScreenSaverEnabled = false;
}

[Serializable]
public struct ControllerPreview
{
    public string ControllerType;
    public Sprite Texture;
}