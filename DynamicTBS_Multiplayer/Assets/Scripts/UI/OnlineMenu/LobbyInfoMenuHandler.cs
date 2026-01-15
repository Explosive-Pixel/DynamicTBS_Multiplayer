using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject joinLobbyButton;
    [SerializeField] private GameObject readyButton;

    [SerializeField] private LobbyDetailsHandler lobbyDetailsHandler;
    [SerializeField] private TimeAndMapInfoHandler timeAndMapInfoHandler;

    [SerializeField] private GameObject nameSetup;

    private Button joinLobby_button;
    private Button ready_button;

    private ISetupHandler nameSetupHandler;

    public Lobby SelectedLobby { get; private set; }

    private void Awake()
    {
        nameSetupHandler = nameSetup.GetComponent<ISetupHandler>();

        MenuEvents.OnChangeLobbySelection += UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby += UpdateInfo;
        MessageReceiver.OnWSMessageReceive += UpdateInfo;

        joinLobby_button = joinLobbyButton.GetComponentInChildren<Button>(true);
        ready_button = readyButton.GetComponentInChildren<Button>(true);

        ready_button.onClick.AddListener(() => SetReady());
        joinLobby_button.onClick.AddListener(() => JoinLobby());
    }

    private void Start()
    {
        if (Client.InLobby)
        {
            OnlineLogicHandler.Instance.LastWSMsgLobbyInfo.HandleMessage();
            UpdateInfo();
        }
    }

    private void Update()
    {
        joinLobby_button.interactable = nameSetupHandler.SetupCompleted;
    }

    private void SetReady()
    {
        AudioEvents.PressingButton();
        Client.IsReady = true;
        Client.SendToServer(new WSMsgSetReady());
    }

    private void JoinLobby()
    {
        if (SelectedLobby == null || !nameSetupHandler.SetupCompleted)
            return;

        AudioEvents.PressingButton();
        Client.JoinLobby(SelectedLobby.LobbyId.FullId, ClientType.PLAYER);
    }

    private void UpdateInfo(Lobby selectedLobby)
    {
        SelectedLobby = selectedLobby;

        if (selectedLobby != null)
        {
            lobbyDetailsHandler.UpdateInfo(selectedLobby);
            timeAndMapInfoHandler.UpdateInfo(selectedLobby?.GameConfig);

            joinLobbyButton.SetActive(!Client.InLobby && SelectedLobby != null && !SelectedLobby.IsFull);

            bool readyCheckAvailable = Client.InLobby && Client.Role == ClientType.PLAYER && Client.CurrentLobby.IsFull;
            if (!readyCheckAvailable)
                Client.IsReady = false;
            ready_button.interactable = readyCheckAvailable && !Client.IsReady;
            readyButton.SetActive(readyCheckAvailable);
        }
        else
        {
            gameObject.GetComponentInParent<BaseActiveHandler>(true).SetInactive();
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
