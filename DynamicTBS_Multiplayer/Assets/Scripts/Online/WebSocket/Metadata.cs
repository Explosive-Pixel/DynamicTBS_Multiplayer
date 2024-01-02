using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metadata : MonoBehaviour
{
    private static int playerCount = 0;
    public static int PlayerCount => playerCount;

    private static int spectatorCount = 0;
    public static int SpectatorCount => spectatorCount;

    private static string pinkName = "pink";
    public static string PinkName => pinkName;

    private static string blueName = "blue";
    public static string BlueName => blueName;

    private void Awake()
    {
        MessageReceiver.OnWSMessageReceive += UpdateMetadata;
    }

    private void UpdateMetadata(WSMessage msg)
    {
        if (msg.code == WSMessageCode.WSMsgMetadataCode)
        {
            WSMsgMetadata msgMetadata = (WSMsgMetadata)msg;
            playerCount = msgMetadata.playerCount;
            spectatorCount = msgMetadata.spectatorCount;
            pinkName = msgMetadata.pinkName;
            blueName = msgMetadata.blueName;
        }
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateMetadata;
    }
}
