using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDraftMessages : MonoBehaviour
{
    [SerializeField] private Text draftMessageText;

    private void Awake()
    {
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
    }

    private void OnDestroy()
    {
        DraftEvents.OnDraftMessageTextChange -= DisplayDraftMessages;
    }
}
