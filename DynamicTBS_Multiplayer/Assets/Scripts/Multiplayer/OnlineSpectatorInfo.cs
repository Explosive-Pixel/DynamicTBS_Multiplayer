using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineSpectatorInfo : MonoBehaviour
{
    [SerializeField] private Text spectatorText;

    private void Awake()
    {
        NetUtility.C_METADATA += UpdateSpectatorText;
        if (GameManager.gameType == GameType.multiplayer)
            DraftEvents.OnStartDraft += InitSpectatorText;

        spectatorText.text = "";
    }

    private void UpdateSpectatorText(NetMessage msg)
    {
        Debug.Log("Update Spectator Count");
        NetMetadata netMetadata = msg as NetMetadata;

        spectatorText.text = "Connected players: " + netMetadata.playerCount;
        if (netMetadata.spectatorCount > 0)
        {
            spectatorText.text += "\nSpectators: " + netMetadata.spectatorCount;
        }
    }

    private void InitSpectatorText()
    {
        spectatorText.text = "Connected players: 2";
    }

    private void OnDestroy()
    {
        NetUtility.C_METADATA -= UpdateSpectatorText;
        DraftEvents.OnStartDraft -= InitSpectatorText;
    }
}
