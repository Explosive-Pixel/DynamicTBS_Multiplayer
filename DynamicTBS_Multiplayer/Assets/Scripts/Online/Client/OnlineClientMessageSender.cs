using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineClientMessageSender : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void SendDraftCharacterMessage(Character character)
    {
        if (OnlineClient.Instance.ShouldSendMessage(character.Side) && character.CharacterType != CharacterType.CaptainChar)
        {
            OnlineClient.Instance.SendToServer(new MsgDraftCharacter
            {
                playerId = character.Side,
                characterType = character.CharacterType
            });
        }
    }

    private void SendPerformActionMessage(ActionMetadata actionMetadata)
    {
        if (OnlineClient.Instance.ShouldSendMessage(actionMetadata.ExecutingPlayer))
        {
            OnlineClient.Instance.SendToServer(new MsgPerformAction
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
        if (OnlineClient.Instance.ShouldSendMessage(player))
        {
            OnlineClient.Instance.SendToServer(new MsgUIAction
            {
                playerId = player,
                uiAction = uiAction
            });
        }
    }

    private void SendUpdateServerMessage(PlayerType currentPlayer)
    {
        if (OnlineClient.Instance.AdminShouldSendMessage())
        {
            OnlineClient.Instance.SendToServer(new MsgUpdateServer
            {
                currentPlayer = currentPlayer,
                gamePhase = GameManager.CurrentGamePhase
            });
        }
    }

    private void SendGameOverMessage(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (OnlineClient.Instance.AdminShouldSendMessage())
        {
            MsgGameOver msg = new MsgGameOver
            {
                isDraw = winner == null
            };

            if (winner != null)
            {
                msg.winner = winner.Value;
            }

            OnlineClient.Instance.SendToServer(msg);
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
