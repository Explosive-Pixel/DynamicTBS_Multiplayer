using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MenuEvents;

public class LobbyDetailsRematchHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyNotAvailableInfo;
    [SerializeField] private GameObject content;

    [SerializeField] private TMPro.TMP_Text lobbyName;
    [SerializeField] private TMPro.TMP_Text lobbyType;

    [SerializeField] private List<TMPro.TMP_Text> playerNames;

    [SerializeField] private TMPro.TMP_Text spectators;

    private Lobby SelectedLobby { get; set; }

    private void Awake()
    {
        if (Client.InLobby)
            UpdateInfo();

        MenuEvents.OnChangeLobbySelection += UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby += UpdateInfo;
        MessageReceiver.OnWSMessageReceive += UpdateInfo;
    }

    public void UpdateInfo(Lobby selectedLobby)
    {
        SelectedLobby = selectedLobby;

        if (content == null || lobbyNotAvailableInfo == null)
            return;

        content.SetActive(selectedLobby != null);
        lobbyNotAvailableInfo.SetActive(selectedLobby == null);

        if (selectedLobby == null)
            return;

        lobbyName.text = selectedLobby.LobbyId.FullId;
        lobbyType.text = selectedLobby.IsPrivate ? "PRIVATE" : "PUBLIC";

        playerNames[0].text = selectedLobby.Players.Count > 0 ? selectedLobby.Players[0].name : "Waiting for player";
        playerNames[1].text = selectedLobby.Players.Count > 1 ? selectedLobby.Players[1].name : "Waiting for player";

        spectators.text = selectedLobby.SpectatorCount.ToString();
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

    private void OnDestroy()
    {
        MenuEvents.OnChangeLobbySelection -= UpdateInfo;
        MenuEvents.OnUpdateCurrentLobby -= UpdateInfo;
        MessageReceiver.OnWSMessageReceive -= UpdateInfo;
    }
}
