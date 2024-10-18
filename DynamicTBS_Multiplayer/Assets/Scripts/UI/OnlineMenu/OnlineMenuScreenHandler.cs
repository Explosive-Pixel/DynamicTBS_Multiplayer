using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuScreenHandler : MonoBehaviour
{
    [SerializeField] private BaseActiveHandler leftActiveHandler;
    [SerializeField] private GameObject lobbyOverviewDisplay;

    [SerializeField] private BaseActiveHandler midActiveHandler;
    [SerializeField] private GameObject lobbyInfoMenu;

    [SerializeField] private BaseActiveHandler rightActiveHandler;
    [SerializeField] private GameObject infoBox;

    public BaseActiveHandler MidActiveHandler { get { return midActiveHandler; } }
    public GameObject LobbyInfoMenu { get { return lobbyInfoMenu; } }

    private void Awake()
    {
        MessageReceiver.OnWSMessageReceive += UpdateMetadata;
    }

    private void UpdateMetadata(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgLobbyInfoCode:
                HandleWSMsgLobbyInfo();
                break;
            case WSMessageCode.WSMsgLobbyListCode:
                HandleWSMsgLobbyList();
                break;
        }
    }

    private void HandleWSMsgLobbyInfo()
    {
        leftActiveHandler.SetInactive();
        midActiveHandler.SetActive(lobbyInfoMenu);
        rightActiveHandler.SetActive(infoBox);
    }

    private void HandleWSMsgLobbyList()
    {
        leftActiveHandler.SetActive(lobbyOverviewDisplay);
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateMetadata;
    }
}
