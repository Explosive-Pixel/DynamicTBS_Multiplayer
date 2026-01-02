using System;

[Serializable]
public class WSMsgLobbyList : WSMessage
{
    public LobbyInfo[] lobbies;
    public int maxLobbyCount;

    public WSMsgLobbyList()
    {
        code = WSMessageCode.WSMsgLobbyListCode;
    }

    public override void HandleMessage()
    {

    }
}
