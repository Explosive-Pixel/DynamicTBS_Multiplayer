using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoMenuHandler : MonoBehaviour
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button readyButton;

    [SerializeField] private LobbyDetailsHandler lobbyDetailsHandler;
    [SerializeField] private TimeAndMapInfoHandler timeAndMapInfoHandler;

    [SerializeField] private GameObject nameSetup;

    private ISetupHandler nameSetupHandler;

    public Lobby SelectedLobby { get; private set; }

    private void Awake()
    {
        nameSetupHandler = nameSetup.GetComponent<ISetupHandler>();

        MenuEvents.OnChangeLobbySelection += UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby += UpdateInfo;
        MessageReceiver.OnWSMessageReceive += UpdateInfo;

        readyButton.onClick.AddListener(() => SetReady());
        joinLobbyButton.onClick.AddListener(() => JoinLobby());
    }

    private void Update()
    {
        joinLobbyButton.interactable = nameSetupHandler.SetupCompleted;
    }

    private void SetReady()
    {
        Client.IsReady = true;
        Client.SendToServer(new WSMsgSetReady());
    }

    private void JoinLobby()
    {
        if (SelectedLobby == null || !nameSetupHandler.SetupCompleted)
            return;

        Client.JoinLobby(SelectedLobby.LobbyId.FullId, ClientType.PLAYER);
    }

    private void UpdateInfo(Lobby selectedLobby)
    {
        SelectedLobby = selectedLobby;

        if (selectedLobby != null)
        {
            lobbyDetailsHandler.UpdateInfo(selectedLobby);
            timeAndMapInfoHandler.UpdateInfo(selectedLobby?.GameConfig);

            joinLobbyButton.gameObject.SetActive(!Client.InLobby && SelectedLobby != null && !SelectedLobby.IsFull);

            bool readyCheckAvailable = Client.InLobby && Client.Role == ClientType.PLAYER && Client.CurrentLobby.IsFull;
            if (!readyCheckAvailable)
                Client.IsReady = false;
            readyButton.interactable = readyCheckAvailable && !Client.IsReady;
            readyButton.gameObject.SetActive(readyCheckAvailable);
        }
    }

    private void UpdateInfo()
    {
        UpdateInfo(Client.CurrentLobby);
    }

    private void UpdateInfo(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgLobbyListCode:
                HandleWSMsgLobbyList((WSMsgLobbyList)msg);
                break;
        }
    }

    private void HandleWSMsgLobbyList(WSMsgLobbyList msg)
    {
        if (SelectedLobby == null)
            return;

        LobbyInfo lobbyInfo = msg.lobbies.ToList().Find(li => li.lobbyId == SelectedLobby.LobbyId.FullId);
        UpdateInfo(lobbyInfo != null ? new Lobby(lobbyInfo) : null);
    }

    private void OnDisable()
    {
        MenuEvents.ChangeLobbySelection(null);
    }

    private void OnDestroy()
    {
        MenuEvents.OnChangeLobbySelection -= UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby -= UpdateInfo;
        MessageReceiver.OnWSMessageReceive -= UpdateInfo;
    }
}
