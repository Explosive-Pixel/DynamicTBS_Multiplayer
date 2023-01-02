using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDraftMessages : MonoBehaviour
{
    [SerializeField] private Text draftMessageText;
    [SerializeField] private Text playerInfoText;

    private void Awake()
    {
        DraftEvents.OnStartDraft += Init;
    }

    private void Init()
    {
        Debug.Log("Initiated draft messages.");
        DraftEvents.OnDraftMessageTextChange += DisplayDraftMessages;
        DisplayDraftMessages();
    }

    private void DisplayDraftMessages()
    {
        int count = DraftManager.GetCurrentDraftCount();
        draftMessageText.text = PlayerManager.GetCurrentPlayer().GetPlayerType() + " selects " + count + " unit";
        if(count > 1)
        {
            draftMessageText.text += "s";
        }
        draftMessageText.text += ".";

        playerInfoText.text = "";
        if (GameManager.gameType == GameType.multiplayer)
        {
            playerInfoText.text = "You are player " + Client.Instance.side + ".";
        }
    }

    private void OnDestroy()
    {
        DraftEvents.OnStartDraft -= Init;
        DraftEvents.OnDraftMessageTextChange -= DisplayDraftMessages;
    }
}
