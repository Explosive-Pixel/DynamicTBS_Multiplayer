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

    private void Start()
    {
        Init();
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
        if (GameManager.gameType == GameType.online)
        {
            if (OnlineClient.Instance.UserData.Role == ClientType.spectator)
            {
                playerInfoText.text = "";
            }
            else
            {
                playerInfoText.text = "You are player " + OnlineClient.Instance.Side + ".";
            }
        }

        pinkDraftOverlay.SetActive(!pinkDraftOverlay.activeSelf);
        blueDraftOverlay.SetActive(pinkDraftOverlay.activeSelf);
    }

    private void OnDestroy()
    {
        DraftEvents.OnDraftMessageTextChange -= DisplayDraftMessages;
    }
}
