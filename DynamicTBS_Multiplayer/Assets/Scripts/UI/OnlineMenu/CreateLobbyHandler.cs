using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyHandler : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private bool newLobby;
    [SerializeField] private GameObject nameSetup;

    private List<ISetupHandler> gameSetup = new();

    private bool SetupCompleted { get { return !gameSetup.Exists(gameSetup => !gameSetup.SetupCompleted); } }

    private bool created = false;

    private void Awake()
    {
        gameSetup = gameObject.GetComponentsInChildren<ISetupHandler>(true).ToList();
        gameSetup.Add(nameSetup.GetComponent<ISetupHandler>());

        createLobbyButton.onClick.AddListener(() => CreateLobby());
        createLobbyButton.interactable = false;

        created = false;
    }

    private void Update()
    {
        createLobbyButton.interactable = SetupCompleted && !created;
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
}
