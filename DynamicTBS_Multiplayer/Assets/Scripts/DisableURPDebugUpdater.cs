using UnityEngine;
using UnityEngine.Rendering;

public class DisableURPDebugUpdater : MonoBehaviour
{
    private void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;

        Debug.LogError(null); // Uncomment this to show console on startup
    }
}

