using System;

[Serializable]
public class WSMsgUpdateServer : WSMessage
{
    public GamePhase gamePhase;
    public PlayerType startPlayer;

    public WSMsgUpdateServer()
    {
        code = WSMessageCode.WSMsgUpdateServerCode;
    }

    public static void SendUpdateServerMessage(GamePhase gamePhase)
    {
        if (GameManager.GameType != GameType.ONLINE || Client.IsLoadingGame)
            return;

        Client.SendToServer(new WSMsgUpdateServer()
        {
            gamePhase = gamePhase,
            startPlayer = PlayerManager.StartPlayer[gamePhase]
        });
    }
}
