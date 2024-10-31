using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoMenuHandler : MonoBehaviour, IExecuteOnSceneLoad
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button readyButton;

    [SerializeField] private LobbyDetailsHandler lobbyDetailsHandler;
    [SerializeField] private TimeAndMapInfoHandler timeAndMapInfoHandler;

    public Lobby SelectedLobby { get; private set; }

    public void ExecuteOnSceneLoaded()
    {
        MenuEvents.OnChangeLobbySelection += UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby += UpdateInfo;
        MessageReceiver.OnWSMessageReceive += UpdateInfo;
    }

    private void Awake()
    {
        readyButton.onClick.AddListener(() => SetReady());
        joinLobbyButton.onClick.AddListener(() => JoinLobby());
    }

    private void SetReady()
    {
        Client.IsReady = true;
        Client.SendToServer(new WSMsgSetReady());
    }

    private void JoinLobby()
    {
        if (SelectedLobby == null)
            return;

        Client.JoinLobby(SelectedLobby.LobbyId.FullId, ClientType.PLAYER);
    }

    private void UpdateInfo(Lobby selectedLobby)
    {
        SelectedLobby = selectedLobby;

        lobbyDetailsHandler.UpdateInfo(selectedLobby);
        timeAndMapInfoHandler.UpdateInfo(selectedLobby?.GameConfig);

        joinLobbyButton.gameObject.SetActive(!Client.InLobby && SelectedLobby != null && !SelectedLobby.IsFull);
        readyButton.interactable = Client.Role == ClientType.PLAYER && !Client.IsReady;
        readyButton.gameObject.SetActive(Client.InLobby && Client.Role == ClientType.PLAYER);
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
        SelectedLobby = null;
    }

    private void OnDestroy()
    {
        MenuEvents.OnChangeLobbySelection -= UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby -= UpdateInfo;
        MessageReceiver.OnWSMessageReceive -= UpdateInfo;
    }
}
