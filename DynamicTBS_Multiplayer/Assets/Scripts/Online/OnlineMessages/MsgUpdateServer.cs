using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgUpdateServer : OnlineMessage
{
    public PlayerType currentPlayer;
    public GamePhase gamePhase;

    public MsgUpdateServer() // Constructing a message.
    {
        Code = OnlineMessageCode.UPDATE_SERVER;
    }

    public MsgUpdateServer(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.UPDATE_SERVER;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte((byte)currentPlayer);
        writer.WriteByte((byte)gamePhase);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        currentPlayer = (PlayerType)reader.ReadByte();
        gamePhase = (GamePhase)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.UpdateGameInfo(LobbyId, currentPlayer, gamePhase);
    }
}
