using UnityEngine;
using System;
using System.IO;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    private string ipAddress;
    public string IpAdress { get { return ipAddress; } }

    private ushort port;
    public ushort Port { get { return port; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadConfig();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");
        string json = File.ReadAllText(path);
        ConfigData data = JsonUtility.FromJson<ConfigData>(json);

        ipAddress = data.ipAddress;
        port = Convert.ToUInt16(data.port);
    }

    [System.Serializable]
    private class ConfigData
    {
        public string ipAddress;
        public int port;
    }
}
