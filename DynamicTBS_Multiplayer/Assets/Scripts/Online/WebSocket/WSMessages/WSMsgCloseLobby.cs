public class WSMsgCloseLobby : WSMessage
{
    public bool regularly;

    public WSMsgCloseLobby()
    {
        code = WSMessageCode.WSMsgCloseLobbyCode;
    }

    public override void HandleMessage()
    {
        Client.LeaveLobby();
    }
}
