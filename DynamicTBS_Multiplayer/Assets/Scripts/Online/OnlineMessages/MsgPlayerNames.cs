using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgPlayerNames : OnlineMessage
{
    public string pinkName;
    public string blueName;

    public MsgPlayerNames() // Constructing a message.
    {
        Code = OnlineMessageCode.PLAYER_NAMES;
    }

    public MsgPlayerNames(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.PLAYER_NAMES;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFixedString32(pinkName);
        writer.WriteFixedString32(blueName);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        pinkName = reader.ReadFixedString32().Value;
        blueName = reader.ReadFixedString32().Value;
    }

    public override void ReceivedOnClient()
    {
        OnlineClient.Instance.UpdatePlayerNames(pinkName, blueName);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}