using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgSyncTimer : OnlineMessage
{
    public float pinkTimeLeft;
    public float blueTimeLeft;
    public int pinkDebuff;
    public int blueDebuff;

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
        writer.WriteInt(pinkDebuff);
        writer.WriteInt(blueDebuff);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        pinkTimeLeft = reader.ReadFloat();
        blueTimeLeft = reader.ReadFloat();
        pinkDebuff = reader.ReadInt();
        blueDebuff = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        GameplayEvents.UpdateTimer(pinkTimeLeft, blueTimeLeft, pinkDebuff, blueDebuff);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}