using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System;

public class MsgUpdateTimer : OnlineMessage
{
    public float pinkTimeLeft;
    public float blueTimeLeft;
    public DateTime startTimestamp;

    public MsgUpdateTimer() // Constructing a message.
    {
        Code = OnlineMessageCode.UPDATE_TIMER;
    }

    public MsgUpdateTimer(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.UPDATE_TIMER;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFloat(pinkTimeLeft);
        writer.WriteFloat(blueTimeLeft);
        string time = startTimestamp.ToString("O");
        writer.WriteFixedString32(time);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        pinkTimeLeft = reader.ReadFloat();
        blueTimeLeft = reader.ReadFloat();
        string time = reader.ReadFixedString32().Value;
        startTimestamp = DateTime.Parse(time).ToUniversalTime();
    }

    public override void ReceivedOnClient()
    {
        GameplayEvents.UpdateTimer(pinkTimeLeft, blueTimeLeft, startTimestamp);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
