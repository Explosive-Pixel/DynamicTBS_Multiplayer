using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerMessageHandler : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome netWelcome = msg as NetWelcome;

        bool isAlreadyRegisteredPlayer = Server.Instance.IsRegisteredPlayer(cnn);
        if (Server.Instance.RegisterClient(cnn, (ClientType)netWelcome.Role))
        {
            Debug.Log("Welcome Server role: " + netWelcome.Role + ", assigned Team: " + netWelcome.AssignedTeam);

            if (netWelcome.Role == (int)ClientType.player)
            {
                if (netWelcome.AssignedTeam != 0)
                {
                    Server.Instance.ReassignSides(cnn, (PlayerType)netWelcome.AssignedTeam);
                    Server.Instance.WelcomePlayers();
                }
                else
                {
                    Server.Instance.WelcomePlayer(cnn);
                }
            }
            else
            {
                Server.Instance.SendToClient(netWelcome, cnn);
            }

            if(!isAlreadyRegisteredPlayer)
            {
                StartCoroutine(Server.Instance.SendGameState(cnn));
            }
        }
    }

    private void OnNetStartGame(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }

    private void OnDraftCharacterServer(NetMessage msg, NetworkConnection cnn)
    {
        NetDraftCharacter netDraftCharacter = msg as NetDraftCharacter;

        Server.Instance.Broadcast(netDraftCharacter);
    }

    private void OnPeformActionServer(NetMessage msg, NetworkConnection cnn)
    {
        NetPerformAction netPerformAction = msg as NetPerformAction;

        Server.Instance.Broadcast(netPerformAction);
    }

    private void OnExecuteUIActionServer(NetMessage msg, NetworkConnection cnn)
    {
        NetExecuteUIAction netExecuteUIAction = msg as NetExecuteUIAction;

        Server.Instance.Broadcast(netExecuteUIAction);
    }

    private void BroadcastExecuteServerAction(ServerActionType serverActionType)
    {
        Server.Instance.Broadcast(new NetExecuteServerAction() { serverActionType = (int)serverActionType });
    }

    private void SwapAdmin(PlayerType? winner, GameOverCondition endGameCondition)
    {
        Server.Instance.SwapAdmin();
        Server.Instance.WelcomePlayers();
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_START_GAME += OnNetStartGame;
        NetUtility.S_DRAFT_CHARACTER += OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION += OnPeformActionServer;
        NetUtility.S_EXECUTE_UIACTION += OnExecuteUIActionServer;

        GameplayEvents.OnExecuteServerAction += BroadcastExecuteServerAction;
        GameplayEvents.OnGameOver += SwapAdmin;
    }

    private void UnsubscribeEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_START_GAME -= OnNetStartGame;
        NetUtility.S_DRAFT_CHARACTER -= OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION -= OnPeformActionServer;
        NetUtility.S_EXECUTE_UIACTION -= OnExecuteUIActionServer;

        GameplayEvents.OnExecuteServerAction -= BroadcastExecuteServerAction;
        GameplayEvents.OnGameOver -= SwapAdmin;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
