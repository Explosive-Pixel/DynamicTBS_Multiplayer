public static class MessageReceiver
{
    public delegate void WSServerMessage(WSMessage msg);
    public static event WSServerMessage OnWSMessageReceive;

    public static void ReceiveMessage(WSMessage msg)
    {
        if (OnWSMessageReceive != null)
            OnWSMessageReceive(msg);
    }
}
