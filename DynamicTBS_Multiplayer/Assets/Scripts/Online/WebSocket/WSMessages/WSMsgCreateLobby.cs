public class WSMsgCreateLobby : WSMessage
{
    public string lobbyName;
    public bool isPrivateLobby;
    public ClientInfo clientInfo;
    public GameConfig gameConfig;

    public WSMsgCreateLobby()
    {
        code = WSMessageCode.WSMsgCreateLobbyCode;
    }
}
