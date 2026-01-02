using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyHandler : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private bool newLobby;
    [SerializeField] private GameObject nameSetup;
    [SerializeField] private GameObject infoBox;

    [SerializeField] private LobbyListHandler lobbyListHandler;

    private List<ISetupHandler> gameSetup = new();

    private bool SetupCompleted { get { return !gameSetup.Exists(gameSetup => !gameSetup.SetupCompleted); } }

    private bool created = false;
    private bool maxLobbyCountReached = false;

    private void Awake()
    {
        if (newLobby)
            maxLobbyCountReached = lobbyListHandler.MaxLobbyCountReached;
        else
            maxLobbyCountReached = false;

        gameSetup = gameObject.GetComponentsInChildren<ISetupHandler>(true).ToList();
        gameSetup.Add(nameSetup.GetComponent<ISetupHandler>());

        createLobbyButton.onClick.AddListener(() => CreateLobby());
        createLobbyButton.interactable = false;

        created = false;

        MessageReceiver.OnWSMessageReceive += UpdateCreateLobbyMenu;
    }

    private void Update()
    {
        if (!newLobby)
        {
            createLobbyButton.interactable = SetupCompleted && !created;
            return;
        }

        createLobbyButton.interactable = SetupCompleted && !created && !maxLobbyCountReached;
        infoBox.SetActive(maxLobbyCountReached);
    }

    private void CreateLobby()
    {
        if (SetupCompleted)
        {
            Client.Role = ClientType.PLAYER;

            if (newLobby)
            {
                SetupLobbyHandler setupLobbyHandler = (SetupLobbyHandler)gameSetup.Find(gameSetup => gameSetup.GetType() == typeof(SetupLobbyHandler));
                Client.CreateLobby(setupLobbyHandler.LobbyName, setupLobbyHandler.IsPrivateLobby);
            }
            else
            {
                Client.ConfigLobby();
            }

            created = true;
        }
    }

    private void UpdateCreateLobbyMenu(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgLobbyListCode:
                HandleWSMsgLobbyList((WSMsgLobbyList)msg);
                break;
            case WSMessageCode.WSMsgServerNotificationCode:
                HandleWSMsgServerNotification((WSMsgServerNotification)msg);
                break;
        }
    }

    private void HandleWSMsgLobbyList(WSMsgLobbyList msg)
    {
        maxLobbyCountReached = msg.lobbies.Count() >= msg.maxLobbyCount;
    }

    private void HandleWSMsgServerNotification(WSMsgServerNotification msg)
    {
        if (msg.serverNotification == WSMsgServerNotification.ServerNotification.LOBBY_CREATION_FORBITTEN_MAX_LOBBY_COUNT_REACHED)
        {
            maxLobbyCountReached = true;
            created = false;
        }
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateCreateLobbyMenu;
    }
}
