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

        if(netWelcome.AssignedTeam != 0)
        {
            Server.Instance.hostSide = netWelcome.AssignedTeam;
            Server.Instance.SendToClient(netWelcome, Server.Instance.connections[0]);

            if(Server.Instance.playerCount > 1)
            {
                Server.Instance.SendToClient(new NetWelcome { AssignedTeam = Server.Instance.GetNonHostSide() }, Server.Instance.connections[1]);
            }
        }

       /* if (Server.Instance.FindConnection(cnn) == -1)
        {
            Server.Instance.playerCount++;
        }*/
        netWelcome.AssignedTeam = Server.Instance.hostSide == 0 ? Server.Instance.playerCount : Server.Instance.GetNonHostSide();
        Debug.Log("Server: Connected players: " + netWelcome.AssignedTeam);

        Server.Instance.SendToClient(netWelcome, cnn);

        /*if(Server.Instance.playerCount == 2)
        {
            Server.Instance.Broadcast(new NetStartGame());
        }*/
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

    #region EventsRegion

    private void SubscribeEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_DRAFT_CHARACTER += OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION += OnPeformActionServer;
    }

    private void UnsubscribeEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_DRAFT_CHARACTER -= OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION -= OnPeformActionServer;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
