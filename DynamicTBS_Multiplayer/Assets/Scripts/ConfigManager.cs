using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    public string Hostname { get; private set; }

    [Serializable]
    private class ConfigData
    {
        public string hostname;
        public string hostname_local;
        public bool local;
    }

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

        Hostname = data.local ? data.hostname_local : data.hostname;
    }
}
