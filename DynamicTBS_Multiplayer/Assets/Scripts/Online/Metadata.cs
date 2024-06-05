using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metadata : MonoBehaviour
{
    public static int PlayerCount { get; private set; } = 0;
    public static int SpectatorCount { get; private set; } = 0;
    public static string PinkName { get; private set; } = "pink";
    public static string BlueName { get; private set; } = "blue";

    private void Awake()
    {
        MessageReceiver.OnWSMessageReceive += UpdateMetadata;
    }

    private void UpdateMetadata(WSMessage msg)
    {
        if (msg.code == WSMessageCode.WSMsgMetadataCode)
        {
            WSMsgMetadata msgMetadata = (WSMsgMetadata)msg;
            PlayerCount = msgMetadata.playerCount;
            SpectatorCount = msgMetadata.spectatorCount;
            PinkName = msgMetadata.pinkName;
            BlueName = msgMetadata.blueName;
        }
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateMetadata;
    }
}
