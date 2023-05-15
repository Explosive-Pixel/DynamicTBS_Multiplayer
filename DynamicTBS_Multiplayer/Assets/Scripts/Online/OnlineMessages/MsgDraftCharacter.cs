using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgDraftCharacter : OnlineMessage
{
    public PlayerType playerId;
    public CharacterType characterType;

    public MsgDraftCharacter() // Constructing a message.
    {
        Code = OnlineMessageCode.DRAFT_CHARACTER;
    }

    public MsgDraftCharacter(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.DRAFT_CHARACTER;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
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
        playerId = (PlayerType)reader.ReadByte();
        characterType = (CharacterType)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        if (OnlineClient.Instance.ShouldReadMessage(playerId))
        {
            DraftManager.DraftCharacter(characterType, PlayerManager.GetPlayer(playerId));
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);

        OnlineServer.Instance.ArchiveCharacterDraft(this);
    }
}
