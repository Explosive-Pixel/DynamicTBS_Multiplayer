using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEvents : MonoBehaviour
{
    public delegate void Message(OnlineMessage msg);
    public static event Message OnMessageReceive;

    public static void ReceiveMessage(OnlineMessage msg)
    {
        if (OnMessageReceive != null)
            OnMessageReceive(msg);
    }
}
