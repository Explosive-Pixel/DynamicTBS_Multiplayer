using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgGameOver : OnlineMessage
{
    public bool isDraw;
    public PlayerType winner;

    public MsgGameOver() // Constructing a message.
    {
        Code = OnlineMessageCode.GAME_OVER;
    }

    public MsgGameOver(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.GAME_OVER;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte(ToByte(isDraw));
        writer.WriteByte((byte)winner);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        isDraw = ToBool(reader.ReadByte());
        winner = (PlayerType)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.GameOver(LobbyId);

        base.ReceivedOnServer(cnn);
    }
}