using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.ComponentModel;

public class CurrentPlayerUIHandler : MonoBehaviour
{
    [SerializeField] private List<GamePhase> associatedGamePhases;

    [SerializeField] private Text playerInfoText;

    [SerializeField] private Text draftMessageText;
    [SerializeField] private Text draftGameSetupText;

    [SerializeField] private Text placementMessageText;

    [SerializeField] private Text gameplayMessageText;

    [SerializeField] private GameObject pinkDraftOverlay;
    [SerializeField] private GameObject blueDraftOverlay;

    private Dictionary<PlayerType, GameObject> overlaysByPlayer = new();

    private void Awake()
    {
        overlaysByPlayer.Add(PlayerType.pink, pinkDraftOverlay);
        overlaysByPlayer.Add(PlayerType.blue, blueDraftOverlay);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        GameplayEvents.OnCurrentPlayerChanged += UpdateMessages;

        if(PlayerManager.GetCurrentPlayer() != null)
        {
            UpdateMessages(PlayerManager.GetCurrentPlayer().GetPlayerType());
        }
    }

    private void UpdateMessages(PlayerType currentPlayer)
    {
        ChangeOverlay(currentPlayer);

        if (!associatedGamePhases.Contains(GameManager.gamePhase))
            return;

        playerInfoText.text = "";
        if (GameManager.gameType == GameType.ONLINE && OnlineClient.Instance.UserData.Role == ClientType.PLAYER)
        {
            playerInfoText.text = "You are player " + OnlineClient.Instance.Side + ".";
        }

        switch (GameManager.gamePhase)
        {
            case GamePhase.DRAFT:
                DisplayDraftMessages(currentPlayer);
                break;
            case GamePhase.PLACEMENT:
                DisplayPlacementMessages(currentPlayer);
                break;
            case GamePhase.GAMEPLAY:
                DisplayGameplayMessages(currentPlayer);
                break;
        }
    }

    private void DisplayDraftMessages(PlayerType currentPlayer)
    {
        int count = DraftManager.GetCurrentDraftCount();
        draftMessageText.text = currentPlayer + " selects " + count + " unit";
        if (count > 1)
        {
            draftMessageText.text += "s";
        }
        draftMessageText.text += ".";

        draftGameSetupText.text = "Map: " + Board.selectedMap.Description() + "\nTime speed: " + Timer.TimerSetupType.Description();
    }

    private void DisplayPlacementMessages(PlayerType currentPlayer)
    {
        int count = PlacementManager.GetCurrentPlacementCount();
        placementMessageText.text = currentPlayer + " places " + count + " unit";
        if (count > 1)
        {
            placementMessageText.text += "s";
        }
        placementMessageText.text += ".";
    }

    private void DisplayGameplayMessages(PlayerType currentPlayer)
    {
        gameplayMessageText.text = "It's " + currentPlayer + "'s turn.";
    }

    private void ChangeOverlay(PlayerType currentPlayer)
    {
        overlaysByPlayer.ToList().ForEach(kv => kv.Value.SetActive(currentPlayer == kv.Key));
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCurrentPlayerChanged -= UpdateMessages;
    }
}
