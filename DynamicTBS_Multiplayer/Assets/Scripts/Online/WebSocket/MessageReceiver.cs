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
        // Debug.Log(rawMsg);
        WSMessage msg = WSMessage.Deserialize(rawMsg);
        ReceiveMessage(msg);
        msg.HandleMessage();
    }
}
