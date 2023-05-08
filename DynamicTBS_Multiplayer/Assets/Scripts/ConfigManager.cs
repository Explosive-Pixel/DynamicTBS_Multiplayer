using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    private string ipAddress;
    public string IpAdress { get { return GetIpAdress(ipAddress); } }

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
        StartCoroutine(LoadStreamingAsset("config.json"));
    }

    private IEnumerator LoadStreamingAsset(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        string result;
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();

            result = www.downloadHandler.text;
        }
        else
            result = File.ReadAllText(filePath);

        ReadJson(result);
    }

    private void ReadJson(string json)
    {
        ConfigData data = JsonUtility.FromJson<ConfigData>(json);

        ipAddress = data.ipAddress;
        port = Convert.ToUInt16(data.port);
    }

    private static string GetIpAdress(string ipAdress)
    {
        if(ipAdress == null)
        {
            Debug.LogError("IP Adress is null");
        }
        return ipAdress;
    }

    [System.Serializable]
    private class ConfigData
    {
        public string ipAddress;
        public int port;
    }
}
