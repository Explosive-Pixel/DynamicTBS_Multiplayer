using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgPerformAction : OnlineMessage
{
    public PlayerType playerId;
    public float characterX;
    public float characterY;
    public ActionType actionType;
    public int actionCount;
    public bool hasDestination;
    public float destinationX;
    public float destinationY;

    public MsgPerformAction() // Constructing a message.
    {
        Code = OnlineMessageCode.PERFORM_ACTION;
    }

    public MsgPerformAction(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.PERFORM_ACTION;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte((byte)playerId);
        writer.WriteFloat(characterX);
        writer.WriteFloat(characterY);
        writer.WriteInt((int)actionType);
        writer.WriteInt(actionCount);
        writer.WriteByte(ToByte(hasDestination));
        writer.WriteFloat(destinationX);
        writer.WriteFloat(destinationY);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        playerId = (PlayerType)reader.ReadByte();
        characterX = reader.ReadFloat();
        characterY = reader.ReadFloat();
        actionType = (ActionType)reader.ReadInt();
        actionCount = reader.ReadInt();
        hasDestination = ToBool(reader.ReadByte());
        destinationX = reader.ReadFloat();
        destinationY = reader.ReadFloat();
    }

    public override void ReceivedOnClient()
    {
        if (OnlineClient.Instance.ShouldReadMessage(playerId))
        {
            Character currentlySelectedCharacter = UIClickHandler.CurrentCharacter;
            GameplayEvents.ChangeCharacterSelection(null);

            if (actionType == ActionType.Skip)
            {
                SkipAction.Execute(actionCount);
            }
            else
            {
                Character character = CharacterManager.GetCharacterByPosition(new Vector3(characterX, characterY, 0));
                if (actionType == ActionType.ActiveAbility)
                {
                    character.ActiveAbility.Execute();
                }
                else
                {
                    ActionUtils.InstantiateAllActionPositions(character);
                }

                if (hasDestination)
                {
                    ActionUtils.ExecuteAction(new Vector3(destinationX, destinationY, 0));
                }
            }

            GameplayEvents.ChangeCharacterSelection(currentlySelectedCharacter);
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);
    }
}
