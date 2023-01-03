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
        spectatorText.text = "";
    }

    private void UpdateSpectatorText(NetMessage msg)
    {
        Debug.Log("Update Spectator Count");
        NetMetadata netMetadata = msg as NetMetadata;

        spectatorText.text = "";
        if (netMetadata.spectatorCount > 0)
        {
            spectatorText.text = "Spectators: " + netMetadata.spectatorCount;
        }
    }

    private void OnDestroy()
    {
        NetUtility.C_METADATA -= UpdateSpectatorText;
    }
}
