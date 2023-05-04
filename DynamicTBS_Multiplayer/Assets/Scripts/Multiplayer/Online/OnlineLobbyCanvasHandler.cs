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

    [SerializeField] private GameObject onlineMetadataCanvas;

    private PlayerType selectedSide;
    private bool sideSelected = false;
    
    private bool AllSelected { get { return sideSelected && gameSetupHandler.AllSelected; } }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        ResetCanvas();
    }

    private void Update()
    {
        UpdateInfoTexts();
    }

    private void UpdateInfoTexts()
    {
        if (!OnlineClient.Instance)
            return;

        switch(OnlineClient.Instance.ConnectionStatus)
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
        }
    }

    private void PrintConnectedInfo()
    {
        SetActive(OnlineClient.Instance.IsAdmin);

        if (OnlineClient.Instance.LobbyId == null)
            return;

        lobbyFullId.text = OnlineClient.Instance.LobbyId.FullId;

        clientInfoText.text = "You are connected!\n";
        if (OnlineClient.Instance.IsAdmin)
        {
            clientInfoText.text += "\nPlease choose a map and a team ";
            if(OnlineClient.Instance.PlayerCount < 2)
            {
                clientInfoText.text += "and wait for another player to connect.";
            } else
            {
                clientInfoText.text += "and start the game.";
            }
        } else
        {
            clientInfoText.text += "\nWaiting for other player to start the game ...";
        }
    }

    private void PrintConnectionDeclinedInfo()
    {
        SetActive(false);

        clientInfoText.text = "The Server refused the connection since there are already two players in the game.\nIf you just disconnected and are trying to reconnect, please try again in a few seconds.";
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
        selectedSide = side;
        sideSelected = true;
    }

    public void StartGame()
    {
        if (AllSelected)
        {
            OnlineClient.Instance.ChooseGameSetup(selectedSide, Board.boardDesignIndex);

            OnlineClient.Instance.SendToServer(new MsgUIAction
            {
                uiAction = UIAction.START_GAME
            });
            OnlineClient.Instance.StartGame();
        }
    }

    private void SetActive(bool active)
    {
        gameSetupHandler.SetActive(active);
        sideSetup.SetActive(active);
        startGameButton.gameObject.SetActive(active);

        if(active)
        {
            startGameButton.interactable = AllSelected && OnlineClient.Instance.PlayerCount == 2;
        }
    }

    private void ShowCanvas()
    {
        ResetCanvas();
        gameObject.SetActive(true);
    }

    private void HideCanvas()
    {
        gameObject.SetActive(false);
    }

    private void ResetCanvas()
    {
        UnsubscribeEvents();
        sideSelected = false;
        selectBlueButton.interactable = true;
        selectPinkButton.interactable = true;
        gameSetupHandler.ResetCanvas();
        SetActive(false);
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        GameEvents.OnGameStart += HideCanvas;
        GameplayEvents.OnRestartGame += ShowCanvas;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGameStart -= HideCanvas;
        GameplayEvents.OnRestartGame -= ShowCanvas;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
