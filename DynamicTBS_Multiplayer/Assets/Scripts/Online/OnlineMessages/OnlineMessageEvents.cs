using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class OnlineMessageEvents
{
    public delegate void Message(OnlineMessage msg, NetworkConnection sender);
    public static event Message OnMessageReceive;

    public static void ReceiveMessage(OnlineMessage msg, NetworkConnection sender)
    {
        if (OnMessageReceive != null)
            OnMessageReceive(msg, sender);
    }
}
