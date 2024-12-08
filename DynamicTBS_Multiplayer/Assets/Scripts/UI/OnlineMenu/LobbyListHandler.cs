using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListHandler : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject lobbyInfoPrefab;

    [SerializeField] private GameObject loadingInfo;
    [SerializeField] private GameObject noLobbiesInfo;

    private void Awake()
    {
        MessageReceiver.OnWSMessageReceive += UpdateLobbyList;
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
            DeleteContent();
            CreateContent(wSMsgLobbyList.lobbies);
            loadingInfo.SetActive(false);
        }
    }

    private void DeleteContent()
    {
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void CreateContent(LobbyInfo[] lobbyList)
    {
        if (lobbyList.Length == 0)
        {
            noLobbiesInfo.SetActive(true);
            return;
        }

        noLobbiesInfo.SetActive(false);
        OnlineMenuScreenHandler onlineMenuScreenHandler = gameObject.GetComponentInParent<OnlineMenuScreenHandler>(true);

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

        foreach (LobbyInfo lobbyInfo in sortedLobbies)
        {
            GameObject lobbyInfoGO = GameObject.Instantiate(lobbyInfoPrefab);
            lobbyInfoGO.transform.SetParent(content.transform);
            lobbyInfoGO.GetComponent<LobbyInfoHandler>().Init(lobbyInfo);
            ChangeActiveGameObjectOnClickHandler clickHandler = lobbyInfoGO.GetComponent<ChangeActiveGameObjectOnClickHandler>();
            clickHandler.activeHandler = onlineMenuScreenHandler.MidActiveHandler;
            clickHandler.activeOnClickGameObject = onlineMenuScreenHandler.LobbyInfoMenu;
            lobbyInfoGO.GetComponent<Button>().onClick.AddListener(() => MenuEvents.ChangeLobbySelection(lobbyInfoGO.GetComponent<LobbyInfoHandler>().Lobby));
        }
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateLobbyList;
    }
}
