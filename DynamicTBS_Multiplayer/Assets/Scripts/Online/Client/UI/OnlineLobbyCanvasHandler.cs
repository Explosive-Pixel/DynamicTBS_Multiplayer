using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OnlineLobbyCanvasHandler : MonoBehaviour
{
    [SerializeField] private Text lobbyFullId;
    [SerializeField] private Text clientInfoText;

    [SerializeField] private GameObject sideSetup;
    [SerializeField] private Button selectPinkButton;
    [SerializeField] private Button selectBlueButton;

    [SerializeField] private Button startGameButton;

    [SerializeField] private GameSetupHandler gameSetupHandler;

    private PlayerType selectedSide;
    private bool sideSelected = false;

    private bool AllSelected { get { return sideSelected && gameSetupHandler.AllSelected; } }

    private bool active = false;

    private void Awake()
    {
        if (GameManager.GameType == GameType.ONLINE)
        {
            active = true;

            startGameButton.onClick.AddListener(() => StartGame());
            SetActive(false);
        }
    }

    private void Update()
    {
        if (!active)
            return;

        UpdateInfoTexts();
    }

    private void UpdateInfoTexts()
    {
        if (!Client.Active)
            return;

        switch (Client.ConnectionStatus)
        {
            case ConnectionStatus.CONNECTED:
                PrintConnectedInfo();
                break;
            case ConnectionStatus.CONNECTION_DECLINED:
                PrintConnectionDeclinedInfo();
                break;
            case ConnectionStatus.LOBBY_NOT_FOUND:
                PrintLobbyNotFoundInfo();
                break;
            case ConnectionStatus.UNCONNECTED:
                PrintUnconnectedInfo();
                break;
            case ConnectionStatus.ATTEMPT_CONNECTION:
                PrintAttemptToConnectInfo();
                break;
            case ConnectionStatus.IN_LOBBY:
                PrintInLobbyInfo();
                break;
        }
    }

    private void PrintInLobbyInfo()
    {
        SetActive(Client.IsAdmin);

        if (Client.LobbyId == null)
            return;

        lobbyFullId.text = Client.LobbyId.FullId;

        clientInfoText.text = "You are connected!\n";

        /* if (OnlineClient.Instance.PlayerCount == 2 && OnlineClient.Instance.OpponentName.Length > 0)
         {
             clientInfoText.text += "Your opponent is " + OnlineClient.Instance.OpponentName + "!\n";
         } */

        if (Client.IsAdmin)
        {
            clientInfoText.text += "\nPlease choose a time speed, a map and a team ";
            if (Metadata.PlayerCount < 2)
            {
                clientInfoText.text += "and wait for another player to connect.";
            }
            else
            {
                clientInfoText.text += "and start the game.";
            }
        }
        else
        {
            if (Client.Role == ClientType.PLAYER)
            {
                clientInfoText.text += "\nWaiting for opponent to start the game ...";
            }
            else
            {
                if (Metadata.PlayerCount < 2)
                {
                    clientInfoText.text += "\nWaiting for player(s) to connect ...";
                }
                else
                {
                    clientInfoText.text += "\nWaiting for player to start the game ...";
                }
            }
        }
    }

    private void PrintConnectedInfo()
    {
        SetActive(false);

        clientInfoText.text = "You are connected to the server!\nTrying to join lobby ...";
    }

    private void PrintConnectionDeclinedInfo()
    {
        SetActive(false);

        clientInfoText.text = "The Server refused the connection since there are already two players in the game.\nIf you want to join as a spectator, please check the respective checkbox before connecting.";
    }

    private void PrintLobbyNotFoundInfo()
    {
        SetActive(false);

        clientInfoText.text = "Connection failed since the requested Lobby does not exist.\nPlease try again with a correct Lobby ID.";
    }

    private void PrintUnconnectedInfo()
    {
        SetActive(false);

        clientInfoText.text = "Could not connect to server. Please try again.";
    }

    private void PrintAttemptToConnectInfo()
    {
        SetActive(false);

        clientInfoText.text = "Trying to connect to server ...";
    }

    public void SelectPink()
    {
        SelectSide(PlayerType.pink);
        selectPinkButton.interactable = false;
        selectBlueButton.interactable = true;
    }

    public void SelectBlue()
    {
        SelectSide(PlayerType.blue);
        selectBlueButton.interactable = false;
        selectPinkButton.interactable = true;
    }

    private void SelectSide(PlayerType side)
    {
        AudioEvents.PressingButton();

        selectedSide = side;
        sideSelected = true;
    }

    public void StartGame()
    {
        if (AllSelected)
        {
            Client.SendStartGameMsg(TimerConfig.DraftAndPlacementTime, TimerConfig.GameplayTime, Board.selectedMapType, selectedSide);
        }
    }

    private void SetActive(bool active)
    {
        gameSetupHandler.SetActive(active);
        sideSetup.SetActive(active);
        startGameButton.gameObject.SetActive(active);

        if (active)
        {
            startGameButton.interactable = AllSelected && Metadata.PlayerCount == 2;
        }
    }
}
