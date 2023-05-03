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
        if (OnlineClient.Instance.ShouldSendMessage(character.GetSide().GetPlayerType()) && character.GetCharacterType() != CharacterType.MasterChar)
        {
            OnlineClient.Instance.SendToServer(new MsgDraftCharacter
            {
                playerId = character.GetSide().GetPlayerType(),
                characterType = character.GetCharacterType()
            });
        }
    }

    private void SendPerformActionMessage(ActionMetadata actionMetadata)
    {
        if (OnlineClient.Instance.ShouldSendMessage(actionMetadata.ExecutingPlayer.GetPlayerType()))
        {
            OnlineClient.Instance.SendToServer(new MsgPerformAction 
            {
                playerId = actionMetadata.ExecutingPlayer.GetPlayerType(),
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

    private void SendUIActionMessage(Player player, UIAction uiAction)
    {
        if (OnlineClient.Instance.ShouldSendMessage(player.GetPlayerType()))
        {
            OnlineClient.Instance.SendToServer(new MsgUIAction 
            {
                playerId = player.GetPlayerType(),
                uiAction = uiAction
            });
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += SendDraftCharacterMessage;
        GameplayEvents.OnFinishAction += SendPerformActionMessage;
        //GameplayEvents.OnExecuteUIAction += SendUIActionMessage;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= SendDraftCharacterMessage;
        GameplayEvents.OnFinishAction -= SendPerformActionMessage;
        //GameplayEvents.OnExecuteUIAction -= SendUIActionMessage;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
