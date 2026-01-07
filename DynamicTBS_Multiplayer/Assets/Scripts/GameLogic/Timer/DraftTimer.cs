using System;

public class DraftTimer : GameTimer
{
    public override void SetActive(DateTime startTime)
    {
        if (!isRunning)
            StartTimer(startTime, originalDuration);
        else
            UnpauseTimer(startTime);
    }

    public override void SetInactive()
    {
        PauseTimer();
    }

    public override void DrawNoTimeLeftConsequences()
    {
        if (GameManager.GameType == GameType.ONLINE && Client.ShouldReadMessage(playerType))
            return;

        DraftManager.StartRandomDraft(playerType);
    }
}
