using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerMessageHandler : MonoBehaviour
{
    [SerializeField] private Text placementMessageText;

    private void Awake()
    {
        PlacementEvents.OnPlacementStart += OnPlacementStart;
        GameplayEvents.OnGameplayPhaseStart += OnGameplayPhaseStart;
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

    private void OnPlacementStart()
    {
        PlacementEvents.OnPlacementMessageChange += DisplayPlacementMessages;
        DisplayPlacementMessages();
    }

    private void OnGameplayPhaseStart()
    {
        PlacementEvents.OnPlacementMessageChange -= DisplayPlacementMessages;
        GameplayEvents.OnPlayerTurnEnded += DisplayTurnMessages;
        DisplayTurnMessages(PlayerManager.GetCurrentPlayer());
    }

    private void OnDestroy()
    {
        PlacementEvents.OnPlacementStart -= OnPlacementStart;
        PlacementEvents.OnPlacementMessageChange -= DisplayPlacementMessages;
        GameplayEvents.OnGameplayPhaseStart -= OnGameplayPhaseStart;
        GameplayEvents.OnPlayerTurnEnded -= DisplayTurnMessages;
    }
}
