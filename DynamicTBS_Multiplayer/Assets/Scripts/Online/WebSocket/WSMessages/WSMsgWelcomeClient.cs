using System;

[Serializable]
public class WSMsgWelcomeClient : WSMessage
{
    public long syncTimestamp;

    public WSMsgWelcomeClient()
    {
        code = WSMessageCode.WSMsgWelcomeClientCode;
    }

    public override void HandleMessage()
    {
        Client.SyncTimeWithServer(syncTimestamp);
    }
}
