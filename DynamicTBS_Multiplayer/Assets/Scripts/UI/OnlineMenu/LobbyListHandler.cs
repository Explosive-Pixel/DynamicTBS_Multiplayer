using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LobbyListHandler : MonoBehaviour, IExecuteOnSceneLoad
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject lobbyInfoPrefab;

    [SerializeField] private GameObject loadingInfo;
    [SerializeField] private GameObject noLobbiesInfo;

    public void ExecuteOnSceneLoaded()
    {
        MessageReceiver.OnWSMessageReceive += UpdateLobbyList;
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
        OnlineMenuScreenHandler onlineMenuScreenHandler = gameObject.GetComponentInParent<OnlineMenuScreenHandler>();

        //LobbyInfo[] sortedData = lobbyList.ToList().OrderBy(lm => lm.playerCount).ToArray();
        foreach (LobbyInfo lobbyInfo in lobbyList)
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
