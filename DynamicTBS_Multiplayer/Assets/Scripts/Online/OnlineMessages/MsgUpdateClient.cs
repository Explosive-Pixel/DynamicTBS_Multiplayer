using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgUpdateClient : OnlineMessage
{
    public bool isAdmin;
    public PlayerType side;
    public string opponentName;

    public MsgUpdateClient() // Constructing a message.
    {
        Code = OnlineMessageCode.UPDATE_CLIENT;
    }

    public MsgUpdateClient(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.UPDATE_CLIENT;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte(ToByte(isAdmin));
        writer.WriteByte((byte)side);
        writer.WriteFixedString32(opponentName);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        isAdmin = ToBool(reader.ReadByte());
        side = (PlayerType)reader.ReadByte();
        opponentName = reader.ReadFixedString32().Value;
    }

    public override void ReceivedOnClient()
    {
        OnlineClient.Instance.UpdateClient(OnlineClient.Instance.LobbyId, isAdmin, side, opponentName);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.AssignSides(LobbyId, cnn, side);
    }
}
