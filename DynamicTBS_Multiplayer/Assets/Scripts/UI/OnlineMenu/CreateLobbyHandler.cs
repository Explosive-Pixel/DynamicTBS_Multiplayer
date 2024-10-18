using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateLobbyHandler : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;

    private List<ISetupHandler> gameSetup = new();

    private bool SetupCompleted { get { return !gameSetup.Exists(gameSetup => !gameSetup.SetupCompleted); } }

    private void Awake()
    {
        gameSetup = gameObject.GetComponentsInChildren<ISetupHandler>(true).ToList();

        createLobbyButton.onClick.AddListener(() => CreateLobby());
        createLobbyButton.interactable = false;
    }

    private void Update()
    {
        createLobbyButton.interactable = SetupCompleted;
    }

    private void CreateLobby()
    {
        if (SetupCompleted)
        {
            Client.Role = ClientType.PLAYER;

            SetupLobbyHandler setupLobbyHandler = (SetupLobbyHandler)gameSetup.Find(gameSetup => gameSetup.GetType() == typeof(SetupLobbyHandler));
            Client.CreateLobby(setupLobbyHandler.LobbyName, setupLobbyHandler.IsPrivateLobby);
        }
    }
}
