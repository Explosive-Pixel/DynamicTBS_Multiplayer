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
        LobbyId = reader.ReadInt();
        isDraw = ToBool(reader.ReadByte());
        winner = (PlayerType)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.SwapAdmin(LobbyId);

        base.ReceivedOnServer(cnn);
    }
}