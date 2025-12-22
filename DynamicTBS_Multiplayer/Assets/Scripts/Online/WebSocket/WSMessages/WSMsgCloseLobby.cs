public class WSMsgCloseLobby : WSMessage
{
    public WSMsgCloseLobby()
    {
        code = WSMessageCode.WSMsgCloseLobbyCode;
    }

    public override void HandleMessage()
    {
        Client.LeaveLobby();
        MenuEvents.CurrentLobbyClosed();
    }
}
