using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading.Tasks;

public static class IPResolver
{
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
}
