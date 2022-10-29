using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDraftMessages : MonoBehaviour
{
    [SerializeField] private Text draftMessageText;
    private int messageCounter;

    private void Awake()
    {
        messageCounter = 0;
        DraftEvents.OnDraftMessageTextChange += DisplayDraftMessages;
        DisplayDraftMessages();
    }

    private void DisplayDraftMessages()
    {
        if (messageCounter == 0)
            draftMessageText.text = "Pink selects 3 units.";
        if (messageCounter == 1)
            draftMessageText.text = "Blue selects 3 units.";
        if (messageCounter == 2)
            draftMessageText.text = "Pink selects 1 unit.";
        if (messageCounter == 3)
            draftMessageText.text = "Blue selects 2 units.";
        if (messageCounter == 4)
            draftMessageText.text = "Pink selects 2 units.";
        if (messageCounter == 5)
            draftMessageText.text = "Blue selects 2 units.";
        if (messageCounter == 6)
            draftMessageText.text = "Pink selects 1 unit.";

        messageCounter += 1;
    }

    private void OnDestroy()
    {
        DraftEvents.OnDraftMessageTextChange -= DisplayDraftMessages;
    }
}
