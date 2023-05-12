using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System;

public class MsgSyncTimer : OnlineMessage
{
    public float pinkTimeLeft;
    public float blueTimeLeft;
    public DateTime startTimestamp;

    public MsgSyncTimer() // Constructing a message.
    {
        Code = OnlineMessageCode.SYNC_TIMER;
    }

    public MsgSyncTimer(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.SYNC_TIMER;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFloat(pinkTimeLeft);
        writer.WriteFloat(blueTimeLeft);
        writer.WriteFixedString64(startTimestamp.ToString("yyyy-MM-ddTHH:mm:sszzz"));
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        pinkTimeLeft = reader.ReadFloat();
        blueTimeLeft = reader.ReadFloat();
        startTimestamp = DateTime.Parse(reader.ReadFixedString64().Value);
    }

    public override void ReceivedOnClient()
    {
        GameplayEvents.UpdateTimer(pinkTimeLeft, blueTimeLeft, startTimestamp);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
