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

        Client.Instance.side = (PlayerType)netWelcome.AssignedTeam;
    }

    private void OnStartGameClient(NetMessage msg)
    {
        GameManager.gameType = GameType.multiplayer;
        GameObject.Find("GameManager").GetComponent<GameManager>().GotToDraftScreen();
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
                playerId = (int)actionMetadata.CharacterInAction.GetSide().GetPlayerType()
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
            Character character = CharacterHandler.GetCharacterByPosition(new Vector3(netPerformAction.characterX, netPerformAction.characterY, 0));
            if(netPerformAction.actionType == (int)ActionType.Skip)
            {
                SkipAction.Execute();
            }
            else if(netPerformAction.actionType == (int)ActionType.ActiveAbility)
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

    #region EventsRegion

    private void SubscribeEvents()
    {
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;

        DraftEvents.OnCharacterCreated += SendDraftCharacterMessage;
        NetUtility.C_DRAFT_CHARACTER += OnDraftCharacter;

        GameplayEvents.OnFinishAction += SendPerformActionMessage;
        NetUtility.C_PERFORM_ACTION += OnPerformAction;
    }

    private void UnsubscribeEvents()
    {
        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;

        DraftEvents.OnCharacterCreated -= SendDraftCharacterMessage;
        NetUtility.C_DRAFT_CHARACTER -= OnDraftCharacter;

        GameplayEvents.OnFinishAction -= SendPerformActionMessage;
        NetUtility.C_PERFORM_ACTION -= OnPerformAction;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
