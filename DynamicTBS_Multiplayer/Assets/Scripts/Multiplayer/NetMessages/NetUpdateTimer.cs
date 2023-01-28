using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUpdateTimer : NetMessage
{

    public float currentTime;
    public int pinkDebuff;
    public int blueDebuff;
    public int currentPlayerId;

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
        writer.WriteInt(pinkDebuff);
        writer.WriteInt(blueDebuff);
        writer.WriteInt(currentPlayerId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        currentTime = reader.ReadFloat();
        pinkDebuff = reader.ReadInt();
        blueDebuff = reader.ReadInt();
        currentPlayerId = reader.ReadInt();
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
