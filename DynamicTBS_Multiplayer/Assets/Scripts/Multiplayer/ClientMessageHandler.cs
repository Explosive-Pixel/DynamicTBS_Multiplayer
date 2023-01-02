using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageHandler : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome netWelcome = msg as NetWelcome;

        if (netWelcome.AssignedTeam != 0)
        {
            Client.Instance.side = (PlayerType)netWelcome.AssignedTeam;
            Debug.Log("Assigned Team " + Client.Instance.side);
        }
    }

    private void OnStartGameClient(NetMessage msg)
    {
        GameManager.gameType = GameType.multiplayer;
        DraftEvents.StartDraft();
        // TODO: Change Camera angle
    }

    private void SendDraftCharacterMessage(Character character)
    {
        if(Client.Instance.side == character.GetSide().GetPlayerType() && character.GetCharacterType() != CharacterType.MasterChar)
        {
            NetDraftCharacter msg = new NetDraftCharacter
            {
                characterType = (int)character.GetCharacterType(),
                playerId = (int)character.GetSide().GetPlayerType()
            };
            Client.Instance.SendToServer(msg);
        }
    }

    private void OnDraftCharacter(NetMessage msg)
    {
        NetDraftCharacter netDraftCharacter = msg as NetDraftCharacter;

        PlayerType playerType = (PlayerType)netDraftCharacter.playerId;
        if (Client.Instance.side != playerType)
        {
            DraftManager.DraftCharacter((CharacterType)netDraftCharacter.characterType, PlayerManager.GetPlayer(playerType));
        }
    }

    private void SendPerformActionMessage(ActionMetadata actionMetadata)
    {
        if (Client.Instance.side == actionMetadata.ExecutingPlayer.GetPlayerType())
        {
            NetPerformAction msg = new NetPerformAction
            {
                characterX = actionMetadata.CharacterInitialPosition != null ? actionMetadata.CharacterInitialPosition.Value.x : 0f,
                characterY = actionMetadata.CharacterInitialPosition != null ? actionMetadata.CharacterInitialPosition.Value.y : 0f,
                actionType = (int)actionMetadata.ExecutedActionType,
                hasDestination = actionMetadata.ActionDestinationPosition != null,
                destinationX = actionMetadata.ActionDestinationPosition != null ? actionMetadata.ActionDestinationPosition.Value.x : 0f,
                destinationY = actionMetadata.ActionDestinationPosition != null ? actionMetadata.ActionDestinationPosition.Value.y : 0f,
                playerId = (int)actionMetadata.ExecutingPlayer.GetPlayerType()
            };
            Client.Instance.SendToServer(msg);
        }
    }

    private void OnPerformAction(NetMessage msg)
    {
        NetPerformAction netPerformAction = msg as NetPerformAction;

        PlayerType playerType = (PlayerType)netPerformAction.playerId;
        if (Client.Instance.side != playerType)
        {
            
            if(netPerformAction.actionType == (int)ActionType.Skip)
            {
                SkipAction.Execute();
                return;
            }

            Character character = CharacterHandler.GetCharacterByPosition(new Vector3(netPerformAction.characterX, netPerformAction.characterY, 0));
            if (netPerformAction.actionType == (int)ActionType.ActiveAbility)
            {
                character.GetActiveAbility().Execute();
            }
            else
            {
                ActionUtils.InstantiateAllActionPositions(character);
            }

            if(netPerformAction.hasDestination)
            {
                Ray ray = UIUtils.DefaultRay(new Vector3(netPerformAction.destinationX, netPerformAction.destinationY, 0));
                ActionUtils.ExecuteAction(ray);
            }
        }
    }

    private void SendExecuteUIActionMessage(Player player, UIActionType uIActionType)
    {
        if (Client.Instance.side == player.GetPlayerType())
        {
            NetExecuteUIAction msg = new NetExecuteUIAction
            {
                uiActionType = (int)uIActionType,
                playerId = (int)player.GetPlayerType()
            };
            Client.Instance.SendToServer(msg);
        }
    }

    private void OnExecuteUIAction(NetMessage msg)
    {
        NetExecuteUIAction netExecuteUIAction = msg as NetExecuteUIAction;

        PlayerType playerType = (PlayerType)netExecuteUIAction.playerId;
        UIActionType uIActionType = (UIActionType)netExecuteUIAction.uiActionType;
        if (Client.Instance.side != playerType)
        {
            GameplayEvents.UIActionExecuted(PlayerManager.GetPlayer(playerType), uIActionType);
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;

        DraftEvents.OnCharacterCreated += SendDraftCharacterMessage;
        NetUtility.C_DRAFT_CHARACTER += OnDraftCharacter;

        GameplayEvents.OnFinishAction += SendPerformActionMessage;
        NetUtility.C_PERFORM_ACTION += OnPerformAction;

        GameplayEvents.OnExecuteUIAction += SendExecuteUIActionMessage;
        NetUtility.C_EXECUTE_UIACTION += OnExecuteUIAction;
    }

    private void UnsubscribeEvents()
    {
        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;

        DraftEvents.OnCharacterCreated -= SendDraftCharacterMessage;
        NetUtility.C_DRAFT_CHARACTER -= OnDraftCharacter;

        GameplayEvents.OnFinishAction -= SendPerformActionMessage;
        NetUtility.C_PERFORM_ACTION -= OnPerformAction;

        GameplayEvents.OnExecuteUIAction -= SendExecuteUIActionMessage;
        NetUtility.C_EXECUTE_UIACTION -= OnExecuteUIAction;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
