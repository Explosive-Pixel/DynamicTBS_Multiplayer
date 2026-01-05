using System;

[Serializable]
public class WSMsgLobbyList : WSMessage
{
    public LobbyInfo[] lobbies;
    public int lobbyCount;
    public int maxLobbyCount;

    public WSMsgLobbyList()
    {
        code = WSMessageCode.WSMsgLobbyListCode;
    }

    public override void HandleMessage()
    {

    }
}
