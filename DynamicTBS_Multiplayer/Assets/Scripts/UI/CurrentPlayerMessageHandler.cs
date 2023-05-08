using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerMessageHandler : MonoBehaviour
{
    [SerializeField] private Text placementMessageText;

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += OnGamePhaseStart;
    }

    private void DisplayPlacementMessages()
    {
        int count = PlacementManager.GetCurrentPlacementCount();
        placementMessageText.text = PlayerManager.GetCurrentPlayer().GetPlayerType() + " places " + count + " unit";
        if(count > 1)
        {
            placementMessageText.text += "s";
        }
        placementMessageText.text += ".";
    }

    private void DisplayTurnMessages(Player player)
    {
        placementMessageText.text = "It's " + PlayerManager.GetOtherPlayer(player).GetPlayerType() + "'s turn.";
    }

    private void OnGamePhaseStart(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.PLACEMENT)
        {
            PlacementEvents.OnPlacementMessageChange += DisplayPlacementMessages;
            DisplayPlacementMessages();
        } else if(gamePhase == GamePhase.GAMEPLAY)
        {
            PlacementEvents.OnPlacementMessageChange -= DisplayPlacementMessages;
            GameplayEvents.OnPlayerTurnEnded += DisplayTurnMessages;
            DisplayTurnMessages(PlayerManager.GetCurrentPlayer());
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= OnGamePhaseStart;
        PlacementEvents.OnPlacementMessageChange -= DisplayPlacementMessages;
        GameplayEvents.OnPlayerTurnEnded -= DisplayTurnMessages;
    }
}
