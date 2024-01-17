using System.Collections;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class IPResult
{
    public string record_type;
    public string value;
}

public static class IPResolver
{
    private class IPResolverMB : MonoBehaviour { }
    private static IPResolverMB instance;

    private static void Init()
    {
        if (instance == null)
        {
            GameObject gameObject = new GameObject("IPResolver");
            instance = gameObject.AddComponent<IPResolverMB>();

            GameObject.Instantiate(gameObject);
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }

    public static string ResolveIp(string givenIp)
    {
        if (IPAddress.TryParse(givenIp, out IPAddress ip))
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return givenIp; // input is a valid IPv4 address
            }
        }
        try
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(givenIp);
            Debug.Log(hostEntry.AddressList.Length);

            foreach (IPAddress address in hostEntry.AddressList)
            {
                Debug.Log(address);
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString(); // input is a valid host address and this is the resolved ip address
                }
            }
        }
        catch (SocketException)
        {
            Debug.LogError("Failed to resolve URL."); // exception means input is not a valid host address
        }

        return null;
    }

    public static async Task<string> GetPublicIpV6Address(string hostname)
    {
        if (IPAddress.TryParse(hostname, out IPAddress ip))
        {
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return hostname; // input is a valid IPv6 address
            }
        }

        using (HttpClient httpClient = new HttpClient())
        {
            string response = await httpClient.GetStringAsync("https://api6.ipify.org?format=json?hostname=" + hostname);
            return response;
        }
    }

    public static void ResolveIpV6(string hostname, Action<string> callback)
    {
        if (IPAddress.TryParse(hostname, out IPAddress ip))
        {
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                callback?.Invoke(hostname); // input is a valid IPv6 address
            }
        }

        Init();

        instance.StartCoroutine(SendRequest("https://api.api-ninjas.com/v1/dnslookup?domain=" + UnityWebRequest.EscapeURL(hostname), callback));
    }

    private static IEnumerator SendRequest(string url, Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("X-Api-Key", "naWZu9pXr5nh+NPJFN3exQ==1WiJ1yM9JOgcGPrY");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                Debug.Log(response);
                var des = JsonConvert.DeserializeObject<IPResult[]>(response);
                callback?.Invoke(des.First(res => res.record_type == "AAAA").value);
            }
            else
            {
                Debug.LogError($"Fehler beim Abrufen der Daten: {webRequest.error}");
                callback?.Invoke(null);
            }

            webRequest.Dispose();
        }
    }
}
