using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetChangeLoadGameStatus : NetMessage
{
    public NetChangeLoadGameStatus() // Constructing a message.
    {
        Code = OperationCode.CHANGE_LOAD_GAME_STATUS;
    }

    public NetChangeLoadGameStatus(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.CHANGE_LOAD_GAME_STATUS;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        // Since the operation code byte has already been read and it's a keep alive message, there's nothing else to read here.
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CHANGE_LOAD_GAME_STATUS?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CHANGE_LOAD_GAME_STATUS?.Invoke(this, cnn);
    }
}