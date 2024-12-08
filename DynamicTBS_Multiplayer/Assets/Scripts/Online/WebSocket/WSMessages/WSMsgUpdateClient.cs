using System;

[Serializable]
public class WSMsgUpdateClient : WSMessage
{
    public float pinkTimeLeft;
    public float blueTimeLeft;
    public long startTimestamp;
    public GamePhase gamePhase;
    public PlayerType currentPlayer;

    public WSMsgUpdateClient()
    {
        code = WSMessageCode.WSMsgUpdateClientCode;
    }

    public override void HandleMessage()
    {
        GameplayEvents.UpdateTimer(pinkTimeLeft, blueTimeLeft, TimerUtils.UnixTimeStampToDateTime(startTimestamp), gamePhase, currentPlayer);
    }
}
