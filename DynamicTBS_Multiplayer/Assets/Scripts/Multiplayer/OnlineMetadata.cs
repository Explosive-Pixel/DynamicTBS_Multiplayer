using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMetadata : MonoBehaviour
{
    [SerializeField] private GameObject multiplayerMetadata;

    private Text infoText;

    private void Awake()
    {
        DontDestroyOnLoad(multiplayerMetadata);
        infoText = multiplayerMetadata.GetComponentInChildren<Text>();

        NetUtility.C_METADATA += UpdateMetadata;

        infoText.text = "";
    }

    private void Update()
    {
        if (Client.Instance && Client.Instance.IsInitialized)
        {
            if (Client.Instance.IsActive)
            {
                if (!Client.Instance.IsConnected)
                {
                    infoText.text = "Lost connection to server. Trying to reconnect ...";
                }
            }
            else
            {
                infoText.text = "Unable to reconnect to server.";
            }
        }
    }

    private void UpdateMetadata(NetMessage msg)
    {
        NetMetadata netMetadata = msg as NetMetadata;

        // Check if Client/Server are active

        infoText.text = "Connected players: " + netMetadata.playerCount;
        if (netMetadata.spectatorCount > 0)
        {
            infoText.text += "\nSpectators: " + netMetadata.spectatorCount;
        }
    }

    private void OnDestroy()
    {
        NetUtility.C_METADATA -= UpdateMetadata;
    }
}
