using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgSyncTimer : OnlineMessage
{
    public PlayerType playerId;
    public CharacterType characterType;

    public MsgSyncTimer() // Constructing a message.
    {
        Code = OnlineMessageCode.SYNC_TIMER;
    }

    public MsgSyncTimer(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.SYNC_TIMER;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte((byte)playerId);
        writer.WriteByte((byte)characterType);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        playerId = (PlayerType)reader.ReadByte();
        characterType = (CharacterType)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        if (Client.Instance.ShouldReadMessage(playerId))
        {
            DraftManager.DraftCharacter(characterType, PlayerManager.GetPlayer(playerId));
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);
    }
}
