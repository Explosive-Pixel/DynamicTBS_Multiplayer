using System.Linq;
using UnityEngine;

public class LobbyListHandler : MonoBehaviour
{
    [SerializeField] private GameObject loadingInfo;
    [SerializeField] private GameObject noLobbiesInfo;

    [SerializeField] private ScrollableLobbyList scrollableLobbyList;
    [SerializeField] private TMPro.TMP_Text lobbyCount;

    public bool MaxLobbyCountReached { get; private set; }

    private Lobby selectedLobby;

    private void Awake()
    {
        MessageReceiver.OnWSMessageReceive += UpdateLobbyList;
        MenuEvents.OnChangeLobbySelection += UpdateSelectedLobby;
    }

    private void OnEnable()
    {
        Client.SendToServer(new WSMsgLobbyList());
    }

    private void UpdateLobbyList(WSMessage msg)
    {
        if (msg.code == WSMessageCode.WSMsgLobbyListCode)
        {
            WSMsgLobbyList wSMsgLobbyList = (WSMsgLobbyList)msg;

            loadingInfo.SetActive(true);
            CreateContent(wSMsgLobbyList.lobbies, wSMsgLobbyList.maxLobbyCount);
            loadingInfo.SetActive(false);
        }
    }

    private void CreateContent(LobbyInfo[] lobbyList, int maxLobbyCount)
    {
        lobbyCount.text = lobbyList.Length + "/" + maxLobbyCount;
        MaxLobbyCountReached = lobbyList.Length >= maxLobbyCount;

        if (lobbyList.Length == 0)
        {
            noLobbiesInfo.SetActive(true);
            scrollableLobbyList.SetLobbies(lobbyList, selectedLobby);
            return;
        }

        noLobbiesInfo.SetActive(false);

        LobbyInfo[] sortedLobbies = lobbyList
            .OrderBy(lobby =>
            {
                return lobby.status switch
                {
                    LobbyStatus.WAITING_FOR_PLAYER => 0,
                    LobbyStatus.UNDER_CONSTRUCTION => 1,
                    LobbyStatus.IN_GAME => 2,
                    _ => 3
                };
            })
            .ToArray();

        scrollableLobbyList.SetLobbies(sortedLobbies, selectedLobby);
    }

    private void UpdateSelectedLobby(Lobby lobby)
    {
        selectedLobby = lobby;
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateLobbyList;
        MenuEvents.OnChangeLobbySelection -= UpdateSelectedLobby;
    }
}
