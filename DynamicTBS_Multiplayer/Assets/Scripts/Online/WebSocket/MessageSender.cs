using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSender : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void SendDraftCharacterMessage(Character character)
    {
        if (Client.ShouldSendMessage(character.Side) && character.CharacterType != CharacterType.CaptainChar)
        {
            Client.SendToServer(new WSMsgDraftCharacter
            {
                playerId = character.Side,
                characterType = character.CharacterType
            });
        }
    }

    private void SendPerformActionMessage(ActionMetadata actionMetadata)
    {
        if (Client.ShouldSendMessage(actionMetadata.ExecutingPlayer))
        {
            Client.SendToServer(new WSMsgPerformAction
            {
                playerId = actionMetadata.ExecutingPlayer,
                characterX = actionMetadata.CharacterInitialPosition != null ? actionMetadata.CharacterInitialPosition.Value.x : 0f,
                characterY = actionMetadata.CharacterInitialPosition != null ? actionMetadata.CharacterInitialPosition.Value.y : 0f,
                actionType = actionMetadata.ExecutedActionType,
                actionCount = actionMetadata.ActionCount,
                hasDestination = actionMetadata.ActionDestinationPosition != null,
                destinationX = actionMetadata.ActionDestinationPosition != null ? actionMetadata.ActionDestinationPosition.Value.x : 0f,
                destinationY = actionMetadata.ActionDestinationPosition != null ? actionMetadata.ActionDestinationPosition.Value.y : 0f,
            });
        }
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

    private void SendUpdateServerMessage(PlayerType currentPlayer)
    {
        if (Client.AdminShouldSendMessage())
        {
            Client.SendToServer(new WSMsgUpdateServer
            {
                currentPlayer = currentPlayer,
                gamePhase = GameManager.CurrentGamePhase
            });
        }
    }

    private void SendGameOverMessage(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (Client.AdminShouldSendMessage())
        {
            Client.SendToServer(new WSMsgGameOver { winner = winner ?? PlayerType.none });
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += SendDraftCharacterMessage;
        GameplayEvents.OnFinishAction += SendPerformActionMessage;
        GameplayEvents.OnExecuteUIAction += SendUIActionMessage;
        GameplayEvents.OnCurrentPlayerChanged += SendUpdateServerMessage;
        GameplayEvents.OnGameOver += SendGameOverMessage;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= SendDraftCharacterMessage;
        GameplayEvents.OnFinishAction -= SendPerformActionMessage;
        GameplayEvents.OnExecuteUIAction -= SendUIActionMessage;
        GameplayEvents.OnCurrentPlayerChanged -= SendUpdateServerMessage;
        GameplayEvents.OnGameOver -= SendGameOverMessage;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
