using Oberon.Remote.Core;
using UnityEngine;
using UnityEngine.UI;

public class OberonStatusUI : MonoBehaviour
{
    public Text IPText;

    private void FixedUpdate()
    {
        var status = ServerStatus.CurrentStatus;
        IPText.text = "IP Address: " + status.ListenIP;
    }
}
