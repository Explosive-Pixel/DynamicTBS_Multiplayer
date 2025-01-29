using System;

[Serializable]
public class WSMsgStartGame : WSMessage
{
    public WSMsgStartGame()
    {
        code = WSMessageCode.WSMsgStartGameCode;
    }

    public override void HandleMessage()
    {
        GameManager.GameType = GameType.ONLINE;
        GameEvents.StartGame();
    }
}
