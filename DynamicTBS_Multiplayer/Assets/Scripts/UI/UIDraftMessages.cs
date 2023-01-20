using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDraftMessages : MonoBehaviour
{
    [SerializeField] private Text draftMessageText;
    [SerializeField] private Text playerInfoText;

    [SerializeField] private GameObject pinkDraftOverlay;
    [SerializeField] private GameObject blueDraftOverlay;

    private void Awake()
    {
        DraftEvents.OnStartDraft += Init;
    }

    private void Init()
    {
        DraftEvents.OnDraftMessageTextChange += DisplayDraftMessages;
        DisplayDraftMessages();
        pinkDraftOverlay.SetActive(true);
        blueDraftOverlay.SetActive(false);
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
            if (Client.Instance.role == ClientType.spectator)
            {
                playerInfoText.text = "";
            }
            else
            {
                playerInfoText.text = "You are player " + Client.Instance.side + ".";
            }
        }

        if (pinkDraftOverlay.activeSelf == true)
        {
            pinkDraftOverlay.SetActive(false);
            blueDraftOverlay.SetActive(true);
        }
        else
        {
            pinkDraftOverlay.SetActive(true);
            blueDraftOverlay.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        DraftEvents.OnStartDraft -= Init;
        DraftEvents.OnDraftMessageTextChange -= DisplayDraftMessages;
    }
}
