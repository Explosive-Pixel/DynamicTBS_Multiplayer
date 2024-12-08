using System;

[Serializable]
public enum UIAction
{
    PAUSE_GAME = 1,
    UNPAUSE_GAME = 2,
    OFFER_DRAW = 3,
    ACCEPT_DRAW = 4,
    DECLINE_DRAW = 5,
    SURRENDER = 6
}

public class WSMsgUIAction : WSMessage
{
    public PlayerType playerId;
    public UIAction uiAction;

    public WSMsgUIAction()
    {
        code = WSMessageCode.WSMsgUIActionCode;
    }

    public override void HandleMessage()
    {
        if (Client.ShouldReadMessage(playerId))
        {
            GameplayEvents.UIActionExecuted(playerId, uiAction);
        }
    }
}
