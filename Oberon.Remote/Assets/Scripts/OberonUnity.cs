using Oberon.Remote.Core;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class OberonUnity : MonoBehaviour 
{
    public UnityInputModule InputModule;
    public static OberonUnity Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InputModule = new UnityInputModule();
        OberonManager.Initialize(InputModule);
    }

    private void Update()
    {
        InputModule.ProcessInput();
    }

    private void OnDestroy()
    {
        OberonManager.SocketServer.Stop();
        Process.GetCurrentProcess().Kill();
    }
}
