using System.Collections;
using System.Collections.Generic;
using System;
using Mono.Cecil.Cil;
using Unity.Networking.Transport;
using UnityEngine;

public class NetPerformAction : NetMessage
{
    // When transporting information through the internet it's important to choose the most basic types possible.
    public float characterX;
    public float characterY;
    public bool activeAbility;
    public bool hasDestination;
    public float destinationX;
    public float destinationY;
    public int playerId;

    public NetPerformAction() // Constructing a message.
    {
        Code = OperationCode.PERFORM_ACTION;
    }

    public NetPerformAction(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.PERFORM_ACTION;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        static int toInt(bool value) { return value == false ? 0 : 1; }

        writer.WriteByte((byte)Code);
        writer.WriteFloat(characterX);
        writer.WriteFloat(characterY);
        writer.WriteInt(toInt(activeAbility));
        writer.WriteInt(toInt(hasDestination));
        writer.WriteFloat(destinationX);
        writer.WriteFloat(destinationY);
        writer.WriteInt(playerId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        static bool toBool(int value) { return value != 0; }

        characterX = reader.ReadFloat();
        characterY = reader.ReadFloat();
        activeAbility = toBool(reader.ReadInt());
        hasDestination = toBool(reader.ReadInt());
        destinationX = reader.ReadFloat();
        destinationY = reader.ReadFloat();
        playerId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_PERFORM_ACTION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_PERFORM_ACTION?.Invoke(this, cnn);
    }
}