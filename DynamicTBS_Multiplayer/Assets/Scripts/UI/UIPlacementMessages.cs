using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlacementMessages : MonoBehaviour
{
    [SerializeField] private Text placementMessageText;
    private int messageCounter;

    private void Awake()
    {
        messageCounter = 0;
        // Event subscription
        DisplayPlacementMessages();
    }

    private void DisplayPlacementMessages()
    {
        if (messageCounter == 0)
            placementMessageText.text = "Pink places 1 unit.";
        if (messageCounter == 1)
            placementMessageText.text = "Blue places 2 units.";
        if (messageCounter == 2)
            placementMessageText.text = "Pink places 2 units.";
        if (messageCounter == 3)
            placementMessageText.text = "Blue places 2 units.";
        if (messageCounter == 4)
            placementMessageText.text = "Pink places 1 unit.";
        if (messageCounter == 5)
            placementMessageText.text = "Blue places 3 units.";
        if (messageCounter == 6)
            placementMessageText.text = "Pink places 3 units.";

        messageCounter += 1;
    }

    private void OnDestroy()
    {
        // Event unsubscription
    }
}
