using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUpdateTimer : NetMessage
{

    public float currentTime;
    public int currentPlayerDebuff;
    public int playerId;

    public NetUpdateTimer() // Constructing a message.
    {
        Code = OperationCode.UPDATE_TIMER;
    }

    public NetUpdateTimer(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.UPDATE_TIMER;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteFloat(currentTime);
        writer.WriteInt(currentPlayerDebuff);
        writer.WriteInt(playerId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        currentTime = reader.ReadFloat();
        currentPlayerDebuff = reader.ReadInt();
        playerId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_UPDATE_TIMER?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_UPDATE_TIMER?.Invoke(this, cnn);
    }
}
