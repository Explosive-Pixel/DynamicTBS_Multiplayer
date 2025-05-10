using UnityEngine;

public class OnlineMenuScreenHandler : MonoBehaviour
{
    [SerializeField] private BaseActiveHandler leftActiveHandler;
    [SerializeField] private GameObject lobbyOverviewDisplay;

    [SerializeField] private BaseActiveHandler midActiveHandler;
    [SerializeField] private GameObject lobbyInfoMenu;
    [SerializeField] private GameObject resetLobbyMenu;
    [SerializeField] private GameObject lobbyInfoRematchMenu;

    [SerializeField] private BaseActiveHandler rightActiveHandler;
    [SerializeField] private GameObject infoBox;
    [SerializeField] private GameObject infoBoxPlayerInfo;

    public BaseActiveHandler MidActiveHandler { get { return midActiveHandler; } }
    public GameObject LobbyInfoMenu { get { return lobbyInfoMenu; } }

    // TODO: Refactor script
    private void Awake()
    {
        MessageReceiver.OnWSMessageReceive += UpdateMetadata;
        MenuEvents.OnUpdateCurrentLobby += ShowRematchMenu;
    }

    private void OnEnable()
    {
        if (!Client.InLobby)
            leftActiveHandler.SetActive(lobbyOverviewDisplay);

        ShowRematchMenu();
    }

    private void UpdateMetadata(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgLobbyInfoCode:
                HandleWSMsgLobbyInfo((WSMsgLobbyInfo)msg);
                break;
        }
    }

    private void HandleWSMsgLobbyInfo(WSMsgLobbyInfo msg)
    {
        if (!Client.InLobby || msg.lobbyInfo.status == LobbyStatus.WAITING_FOR_PLAYER)
            ShowLobbyInfoMenu();
        else
            ShowRematchMenu();
    }

    private void ShowLobbyInfoMenu()
    {
        leftActiveHandler.SetInactive();
        midActiveHandler.SetActive(lobbyInfoMenu);
        rightActiveHandler.SetActive(infoBoxPlayerInfo);
    }

    private void ShowRematchMenu()
    {
        if (!Client.InLobby || Client.CurrentLobby.Status != LobbyStatus.UNDER_CONSTRUCTION)
            return;

        leftActiveHandler.SetInactive();
        if (Client.IsAdmin)
        {
            midActiveHandler.SetActive(resetLobbyMenu);
            rightActiveHandler.SetActive(infoBox);
        }
        else
        {
            midActiveHandler.SetActive(lobbyInfoRematchMenu);
            rightActiveHandler.SetInactive();
        }
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateMetadata;
        MenuEvents.OnUpdateCurrentLobby -= ShowRematchMenu;
    }
}
