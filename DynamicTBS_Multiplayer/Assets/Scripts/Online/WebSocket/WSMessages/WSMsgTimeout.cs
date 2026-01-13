using System;

[Serializable]
public class WSMsgTimeout : WSMessage
{
    public WSMsgTimeout()
    {
        code = WSMessageCode.WSMsgTimeoutCode;
    }

    public override void HandleMessage()
    {
        GameplayEvents.TimerTimedOut(GameManager.CurrentGamePhase, PlayerManager.CurrentPlayer);
    }
}
