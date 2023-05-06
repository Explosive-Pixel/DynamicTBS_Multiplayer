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
        GameEvents.OnGamePhaseStart += Init;
    }

    private void Init(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.DRAFT)
        {
            Init();
        }
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
        GameEvents.OnGamePhaseStart -= Init;
        DraftEvents.OnDraftMessageTextChange -= DisplayDraftMessages;
    }
}
