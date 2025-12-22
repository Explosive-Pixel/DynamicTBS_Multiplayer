using UnityEngine;

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
        Debug.Log("raw: " + rawMsg);
        WSMessage msg = WSMessage.Deserialize(rawMsg);

        if (msg != null)
        {
            Debug.Log(msg);
            ReceiveMessage(msg);
            msg.HandleMessage();
        }
        else
        {
            Debug.Log("Message '" + rawMsg + "' could not be serialized.");
        }
    }
}
