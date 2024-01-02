using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using NativeWebSocket;

public static class MessageReceiver
{
    public delegate void WSServerMessage(WSMessage msg);
    public static event WSServerMessage OnWSMessageReceive;

    public static void ReceiveMessage(WSMessage msg)
    {
        if (OnWSMessageReceive != null)
            OnWSMessageReceive(msg);
    }

    public static void ReceiveMessage(string rawMsg)
    {
        WSMessage msg = WSMessage.Deserialize(rawMsg);
        ReceiveMessage(msg);
        msg.HandleMessage();
    }
}
