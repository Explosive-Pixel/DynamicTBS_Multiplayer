using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgWelcomeClient : OnlineMessage
{
    public string lobbyName;
    public bool isAdmin;

    public MsgWelcomeClient() // Constructing a message.
    {
        Code = OnlineMessageCode.WELCOME_CLIENT;
    }

    public MsgWelcomeClient(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.WELCOME_CLIENT;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFixedString32(lobbyName);
        writer.WriteByte(ToByte(isAdmin));
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        lobbyName = reader.ReadFixedString32().ToString();
        isAdmin = ToBool(reader.ReadByte());
    }

    public override void ReceivedOnClient()
    {
        LobbyId lobbyId = new LobbyId(LobbyId, lobbyName);
        OnlineClient.Instance.UpdateClient(lobbyId, isAdmin, OnlineClient.Instance.Side);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
