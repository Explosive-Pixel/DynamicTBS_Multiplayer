using UnityEngine;
using UnityEngine.Rendering;

public static class DisableURPDebug
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DisableDebug()
    {
        if (DebugManager.instance != null)
        {
            DebugManager.instance.enableRuntimeUI = false;
        }
    }
}

