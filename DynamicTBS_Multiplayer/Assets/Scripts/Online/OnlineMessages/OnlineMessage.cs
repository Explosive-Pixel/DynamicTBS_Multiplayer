using System;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class OnlineMessage
{
    public OnlineMessageCode Code { set; get; }
    public int LobbyId { set; get; } // ShortId of class LobbyId

    public virtual void Serialize(ref DataStreamWriter writer, int lobbyId) // Packaging message for sending.
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(lobbyId);
    }

    public virtual void Deserialize(DataStreamReader reader) // Unpackaging and distributing message after receiving.
    {
        LobbyId = reader.ReadInt();
    }

    public virtual void ReceivedOnClient() // Called when message is received on client side.
    {

    }

    public virtual void ReceivedOnServer(NetworkConnection cnn) // Called when message is received on server side, with identifier from whom it was sent.
    {
        ServerEvents.ReceiveMessage(this);
    }

    public static byte ToByte(bool value) { return value == false ? (byte)0 : (byte)1; }
    public static bool ToBool(byte value) { return value == 1; }
}