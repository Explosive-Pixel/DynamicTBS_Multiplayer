using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System;

public class MsgSyncTime : OnlineMessage
{
    public DateTime timestamp;

    public MsgSyncTime() // Constructing a message.
    {
        Code = OnlineMessageCode.SYNC_TIME;
    }

    public MsgSyncTime(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.SYNC_TIME;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFixedString32(timestamp.ToString("O"));
    }

    public override void Deserialize(DataStreamReader reader)
    {
        timestamp = DateTime.Parse(reader.ReadFixedString32().Value).ToUniversalTime();
    }

    public override void ReceivedOnClient()
    {
        OnlineClient.Instance.ServerTimeDiff = TimerUtils.TimeSince(timestamp);
        Debug.Log("Synchronized Time with Server. Difference is: " + OnlineClient.Instance.ServerTimeDiff);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.SendToClient(new MsgSyncTime
        {
            timestamp = TimerUtils.Timestamp()
        }, cnn, LobbyId);
    }
}
