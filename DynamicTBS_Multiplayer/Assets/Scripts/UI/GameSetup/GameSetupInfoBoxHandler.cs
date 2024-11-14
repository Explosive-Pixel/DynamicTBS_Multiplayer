using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupInfoBoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject waitingForPlayerInfo;
    [SerializeField] private GameObject readyPlayersInfo;

    private void Update()
    {
        if (!Client.InLobby || Client.CurrentLobby.Status == LobbyStatus.UNDER_CONSTRUCTION)
            return;

        BaseActiveHandler activeHandler = gameObject.GetComponent<BaseActiveHandler>();
        if (Client.CurrentLobby.PlayerCount < 2)
        {
            activeHandler.SetActive(waitingForPlayerInfo);
        }
        else
        {
            activeHandler.SetActive(readyPlayersInfo);
        }
    }
}
