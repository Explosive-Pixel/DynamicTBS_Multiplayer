using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

public static class Telegram
{
    private const bool active = false;

    private static string chat_id = "288226916"; // ID (you can know your id via @userinfobot)
    private static string TOKEN = "6216484760:AAF8oUP5UZkztNuY7pEmJ2uTnMR853yIxww"; // bot token (@BotFather)
    private static string API_URL
    {
        get
        {
            return string.Format("https://api.telegram.org/bot{0}/", TOKEN);
        }
    }

    private class TelegramMB : MonoBehaviour { }
    private static TelegramMB instance;

    private static void Init()
    {
        if (instance == null)
        {
            GameObject gameObject = new GameObject("Telegram");
            instance = gameObject.AddComponent<TelegramMB>();
        }
    }

    public static void SendFile(byte[] bytes, string filename, string caption = "")
    {
        if (!active)
            return;

        Init();

        WWWForm form = new WWWForm();
        form.AddField("chat_id", chat_id);
        form.AddField("caption", caption);
        form.AddBinaryData("document", bytes, filename, "filename");
        UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendDocument?", form);

        instance.StartCoroutine(SendRequest(www));
    }

    private static IEnumerator SendRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        www.Dispose();
    }

    public static void SendMessage(string text)
    {
        if (!active)
            return;

        string json = "{\"chat_id\":\"" + chat_id + "\",\"text\":\"" + text + "\"}";
        HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        Task.Run(() => SendRequest(API_URL + "sendMessage?", httpContent));
    }

    private static async void SendRequest(string url, HttpContent body)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.PostAsync(url, body);

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError(response.StatusCode.ToString());
            }
        }
    }
}