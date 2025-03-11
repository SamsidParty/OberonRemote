using Oberon.Remote.Core;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class OberonUnity : MonoBehaviour 
{
    public UnityInputModule InputModule;
    public static OberonUnity Instance;

    private void Awake()
    {
        
        #if UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = 120;
        #else 
        Application.targetFrameRate = -1; // Disables Vsync
        #endif

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InputModule = new UnityInputModule();
        OberonManager.Initialize(InputModule);

        if (Environment.MachineName == "localhost")
        {
            ServerStatus.CurrentStatus.MachineName = SystemInfo.deviceName;
        }
        else
        {
            ServerStatus.CurrentStatus.MachineName = Environment.MachineName;
        }
    }

    private void Update()
    {
        InputModule.ProcessInput();
    }

    private void OnDestroy()
    {
        OberonManager.SocketServer.Stop();

#if UNITY_STANDALONE_WIN
        Process.GetCurrentProcess().Kill();
#endif
    }
}
