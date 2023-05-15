using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

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
            IPAddress[] addresses = Dns.GetHostAddresses(givenIp);
            if (addresses.Length > 0)
            {
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.ToString(); // input is a valid host address and this is the resolved ip address
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to resolve URL.");
            }
        }
        catch (SocketException)
        {
            Debug.LogError("Failed to resolve URL."); // exception means input is not a valid host address
        }

        return null;
    }
}
