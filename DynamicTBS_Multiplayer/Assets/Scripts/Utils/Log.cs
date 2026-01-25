using UnityEngine;

public static class Log
{
    public static void Info(string msg)
    {
        if (ConfigManager.Instance != null && ConfigManager.Instance.DebugEnabled)
            Debug.Log(msg);
    }

    public static void Warn(string msg)
    {
        Debug.LogWarning(msg);
    }

    public static void Error(string msg)
    {
        Debug.LogError(msg);
    }
}

