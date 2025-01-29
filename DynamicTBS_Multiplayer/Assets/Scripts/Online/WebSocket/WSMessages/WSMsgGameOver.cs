using System;

[Serializable]
public class WSMsgGameOver : WSMessage
{
    public PlayerType winner;
    public GameOverCondition gameOverCondition;

    public WSMsgGameOver()
    {
        code = WSMessageCode.WSMsgGameOverCode;
    }
}
