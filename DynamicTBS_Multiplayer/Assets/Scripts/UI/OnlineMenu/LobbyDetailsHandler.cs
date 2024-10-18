using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LobbyDetailsHandler : MonoBehaviour, IExecuteOnSceneLoad
{
    [SerializeField] private GameObject lobbyNotAvailableInfo;
    [SerializeField] private GameObject content;

    [SerializeField] private TMPro.TMP_Text lobbyName;
    [SerializeField] private TMPro.TMP_Text lobbyType;

    [SerializeField] private TMPro.TMP_Text bluePlayerName;
    [SerializeField] private TMPro.TMP_Text pinkPlayerName;
    [SerializeField] private GameObject waitingForBluePlayer;
    [SerializeField] private GameObject waitingForPinkPlayer;

    [SerializeField] private TMPro.TMP_Text spectators;
    [SerializeField] private Button joinAsSpectatorButton;

    public Lobby SelectedLobby { get; private set; }

    public void ExecuteOnSceneLoaded()
    {
        MenuEvents.OnChangeLobbySelection += UpdateInfo;
        MessageReceiver.OnWSMessageReceive += UpdateInfo;
    }

    private void Awake()
    {
        joinAsSpectatorButton.onClick.AddListener(() => JoinLobby());
    }

    /*private void Update()
    {
        if (!Client.InLobby)
            return;

        UpdateInfo(Client.CurrentLobby);
    }*/

    public void UpdateInfo(Lobby selectedLobby)
    {
        SelectedLobby = selectedLobby;
        content.SetActive(SelectedLobby != null);
        lobbyNotAvailableInfo.SetActive(SelectedLobby == null);

        if (SelectedLobby == null)
            return;

        lobbyName.text = SelectedLobby.LobbyId.FullId;
        lobbyType.text = SelectedLobby.IsPrivate ? "PRIVATE" : "PUBLIC";

        HandlePlayerText(PlayerType.blue);
        HandlePlayerText(PlayerType.pink);

        spectators.text = SelectedLobby.SpectatorCount.ToString();
        joinAsSpectatorButton.gameObject.SetActive(!Client.InLobby);
    }

    private void HandlePlayerText(PlayerType side)
    {
        ClientInfo player = SelectedLobby.GetPlayer(side);

        if (player == null)
        {
            if (side == PlayerType.blue)
            {
                bluePlayerName.gameObject.SetActive(false);
                waitingForBluePlayer.SetActive(true);
            }
            else
            {
                pinkPlayerName.gameObject.SetActive(false);
                waitingForPinkPlayer.SetActive(true);
            }
        }
        else
        {
            if (side == PlayerType.blue)
            {
                bluePlayerName.gameObject.SetActive(true);
                bluePlayerName.text = player.name;
                waitingForBluePlayer.SetActive(false);
            }
            else
            {
                pinkPlayerName.gameObject.SetActive(true);
                pinkPlayerName.text = player.name;
                waitingForPinkPlayer.SetActive(false);
            }
        }

    }

    private void UpdateInfo(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgLobbyInfoCode:
                UpdateInfo(new Lobby(((WSMsgLobbyInfo)msg).lobbyInfo));
                break;
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

    private void JoinLobby()
    {
        if (SelectedLobby == null)
            return;

        Client.JoinLobby(SelectedLobby.LobbyId.FullId, ClientType.SPECTATOR);
    }

    private void OnDisable()
    {
        SelectedLobby = null;
    }

    private void OnDestroy()
    {
        MenuEvents.OnChangeLobbySelection -= UpdateInfo;
        MessageReceiver.OnWSMessageReceive -= UpdateInfo;
    }
}
