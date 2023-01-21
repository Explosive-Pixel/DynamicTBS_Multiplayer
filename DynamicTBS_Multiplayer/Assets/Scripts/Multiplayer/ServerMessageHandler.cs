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

        Server.Instance.RegisterAs(cnn, (ClientType)netWelcome.Role);
        Debug.Log("Welcome Server role: " + netWelcome.Role + ", assigned Team: " + netWelcome.AssignedTeam);

        if (netWelcome.Role == (int)ClientType.player)
        {
            netWelcome.isAdmin = Server.Instance.IsAdmin(cnn);

            if (netWelcome.AssignedTeam != 0)
            {
                Server.Instance.ChosenSide = netWelcome.AssignedTeam;
                Server.Instance.SendToClient(netWelcome, cnn);

                NetworkConnection? otherConnection = Server.Instance.FindOtherPlayer(cnn);
                if (otherConnection != null)
                {
                    Server.Instance.SendToClient(new NetWelcome() { AssignedTeam = Server.Instance.GetOtherSide(), Role = netWelcome.Role}, otherConnection.Value);
                }
            }
            else
            {
                netWelcome.AssignedTeam = Server.Instance.ChosenSide == 0 ? Server.Instance.PlayerCount : Server.Instance.GetOtherSide();
                Server.Instance.SendToClient(netWelcome, cnn);
            }
        } else
        {
            Server.Instance.SendToClient(netWelcome, cnn);
        }

        StartCoroutine(Server.Instance.SendGameState(cnn));
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

    private void SwapAdmin(PlayerType? winner)
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

        GameplayEvents.OnGameOver += SwapAdmin;
    }

    private void UnsubscribeEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_START_GAME -= OnNetStartGame;
        NetUtility.S_DRAFT_CHARACTER -= OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION -= OnPeformActionServer;
        NetUtility.S_EXECUTE_UIACTION -= OnExecuteUIActionServer;

        GameplayEvents.OnGameOver -= SwapAdmin;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
