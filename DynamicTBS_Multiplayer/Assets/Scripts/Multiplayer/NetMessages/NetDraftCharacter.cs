using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetDraftCharacter : NetMessage
{
    public int characterType;
    public int playerId;

    public NetDraftCharacter() // Constructing a message.
    {
        Code = OperationCode.DRAFT_CHARACTER;
    }

    public NetDraftCharacter(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.DRAFT_CHARACTER;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(characterType);
        writer.WriteInt(playerId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        characterType = reader.ReadInt();
        playerId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_DRAFT_CHARACTER?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_DRAFT_CHARACTER?.Invoke(this, cnn);
    }
}
