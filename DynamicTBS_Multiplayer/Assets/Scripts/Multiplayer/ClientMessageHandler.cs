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
        Debug.Log("Client: Welcome");
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

    private void SendPerformActionMessage(Character character, ActionType actionType, Vector3 characterInitialPosition, Vector3? actionDestinationPosition)
    {
        if (Client.Instance.side == character.GetSide().GetPlayerType()) // TODO: Could character be turned by Master's passive -> problem?
        {
            Debug.Log("CharacterInitialPosition: (x: " + characterInitialPosition.x + ", y: " + characterInitialPosition.y + ")");
            NetPerformAction msg = new NetPerformAction
            {
                characterX = characterInitialPosition.x,
                characterY = characterInitialPosition.y,
                activeAbility = actionType == ActionType.ActiveAbility,
                hasDestination = actionDestinationPosition != null,
                destinationX = actionDestinationPosition != null ? actionDestinationPosition.Value.x : 0f,
                destinationY = actionDestinationPosition != null ? actionDestinationPosition.Value.y : 0f,
                playerId = (int)character.GetSide().GetPlayerType()
            };
            Client.Instance.SendToServer(msg);
        }
    }

    private void OnPerformAction(NetMessage msg)
    {
        NetPerformAction netPerformAction = msg as NetPerformAction;

        Debug.Log("Client: Received NetPerformAction");

        PlayerType playerType = (PlayerType)netPerformAction.playerId;
        if (Client.Instance.side != playerType)
        {
            Debug.Log("CharacterInitialPosition: (x: " + netPerformAction.characterX + ", y: " + netPerformAction.characterY + ")");
            Character character = CharacterHandler.GetCharacterByPosition(new Vector3(netPerformAction.characterX, netPerformAction.characterY, 0));
            Debug.Log("Client: Character in Action: " + character);
            if(netPerformAction.activeAbility)
            {
                character.GetActiveAbility().Execute();
                Debug.Log("Client: Execute Active ability");
            }
            else
            {
                ActionUtils.InstantiateAllActionPositions(character);
                Debug.Log("Client: InstantiateActionPosition");
            }

            if(netPerformAction.hasDestination)
            {
                Ray ray = UIUtils.DefaultRay(new Vector3(netPerformAction.destinationX, netPerformAction.destinationY, 0));
                ActionUtils.ExecuteAction(ray);
                Debug.Log("Client: Execute Action");
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
