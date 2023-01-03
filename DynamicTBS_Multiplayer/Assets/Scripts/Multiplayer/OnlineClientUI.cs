using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineClientUI : MonoBehaviour
{
    [SerializeField] private Text clientInfoText;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button selectPinkButton;
    [SerializeField] private Button selectBlueButton;
    [SerializeField] private Text connectedPlayers;

    private bool sideSelected = false;
    private int connectedPlayersCount = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        SubscribeEvents();

        selectPinkButton.gameObject.SetActive(false);
        selectBlueButton.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateInfoTexts();
    }

    public void OnSelectPink()
    {
        OnSelectSide(PlayerType.pink);
        selectPinkButton.interactable = false;
        selectBlueButton.gameObject.SetActive(false);
    }

    public void OnSelectBlue()
    {
        OnSelectSide(PlayerType.blue);
        selectBlueButton.interactable = false;
        selectPinkButton.gameObject.SetActive(false);
    }

    private void OnSelectSide(PlayerType side)
    {
        Client.Instance.SendToServer(new NetWelcome() { AssignedTeam = (int)side, Role = (int)Client.Instance.role, isAdmin = true });
        sideSelected = true;
    }

    public void OnStartGame()
    {
        Client.Instance.SendToServer(new NetStartGame());
    }

    private void UpdatePlayerCount(NetMessage msg)
    {
        NetMetadata netMetadata = msg as NetMetadata;

        connectedPlayersCount = netMetadata.playerCount;
    }

    private void UpdateInfoTexts()
    {
        if (Client.Instance.IsConnected)
        {
            clientInfoText.text = "You are connected!\n";
            if (!Client.Instance.isAdmin)
            {
                clientInfoText.text += "\nWaiting for other player to start the game ...";
            }
            else 
            {
                clientInfoText.text += "\nPlease choose a team ";
                if (connectedPlayersCount < 2)
                {
                    clientInfoText.text += "and wait for another player to connect.";
                }
                else
                {
                    clientInfoText.text += "and start the game.";
                }

                selectPinkButton.gameObject.SetActive(true);
                selectBlueButton.gameObject.SetActive(true);
            }
        }
        else if (Client.Instance.IsActive)
        {
            clientInfoText.text = "Trying to connect to host ...";
        }
        else
        {
            clientInfoText.text = "Could not connect to host. Please try again.";
        }

        connectedPlayers.text = "Connected players: " + connectedPlayersCount;
        if (connectedPlayersCount == 2 && sideSelected)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private void HideCanvas(NetMessage msg)
    {
        gameObject.SetActive(false);
    }

    private void ShowCanvas()
    {
        gameObject.SetActive(true);
        sideSelected = false;
        selectPinkButton.interactable = true;
        selectBlueButton.interactable = true;
        startGameButton.gameObject.SetActive(false);
    }

    private void SubscribeEvents()
    {
        NetUtility.C_METADATA += UpdatePlayerCount;
        NetUtility.C_START_GAME += HideCanvas;
        GameplayEvents.OnRestartGame += ShowCanvas;
    }

    private void OnDestroy()
    {
        NetUtility.C_METADATA -= UpdatePlayerCount;
        NetUtility.C_START_GAME -= HideCanvas;
        GameplayEvents.OnRestartGame -= ShowCanvas;
    }
}
