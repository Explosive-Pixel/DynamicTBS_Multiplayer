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
            if (netWelcome.AssignedTeam != 0)
            {
                Server.Instance.HostSide = netWelcome.AssignedTeam;
                Server.Instance.SendToClient(netWelcome, cnn);

                NetworkConnection? otherConnection = Server.Instance.FindOtherConnection(cnn);
                if (otherConnection != null)
                {
                    Server.Instance.SendToClient(new NetWelcome() { AssignedTeam = Server.Instance.GetNonHostSide(), Role = netWelcome.Role}, otherConnection.Value);
                }
            }
            else
            {
                netWelcome.AssignedTeam = Server.Instance.HostSide == 0 ? Server.Instance.PlayerCount : Server.Instance.GetNonHostSide();
                Server.Instance.SendToClient(netWelcome, cnn);
            }
        } else
        {
            Server.Instance.SendToClient(netWelcome, cnn);
        }
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

    #region EventsRegion

    private void SubscribeEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_DRAFT_CHARACTER += OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION += OnPeformActionServer;
        NetUtility.S_EXECUTE_UIACTION += OnExecuteUIActionServer;
    }

    private void UnsubscribeEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_DRAFT_CHARACTER -= OnDraftCharacterServer;
        NetUtility.S_PERFORM_ACTION -= OnPeformActionServer;
        NetUtility.S_EXECUTE_UIACTION -= OnExecuteUIActionServer;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
