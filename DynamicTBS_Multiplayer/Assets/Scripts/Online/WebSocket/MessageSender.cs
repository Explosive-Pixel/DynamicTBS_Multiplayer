using UnityEngine;

public class MessageSender : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void SendUIActionMessage(PlayerType player, UIAction uiAction)
    {
        if (Client.ShouldSendMessage(player))
        {
            if (uiAction == UIAction.PAUSE_GAME || uiAction == UIAction.UNPAUSE_GAME)
            {
                SendPauseGameMessage(uiAction == UIAction.PAUSE_GAME);
                return;
            }

            Client.SendToServer(new WSMsgUIAction
            {
                playerId = player,
                uiAction = uiAction
            });
        }
    }

    private void SendPauseGameMessage(bool paused)
    {
        Client.SendToServer(new WSMsgPauseGame
        {
            pause = paused
        });
    }

    private void SendGameOverMessage(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (Client.Role != ClientType.PLAYER || Client.IsLoadingGame)
            return;

        Client.SendToServer(new WSMsgGameOver { winner = winner ?? PlayerType.none, gameOverCondition = endGameCondition });
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnExecuteUIAction += SendUIActionMessage;
        GameplayEvents.OnGameOver += SendGameOverMessage;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnExecuteUIAction -= SendUIActionMessage;
        GameplayEvents.OnGameOver -= SendGameOverMessage;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
