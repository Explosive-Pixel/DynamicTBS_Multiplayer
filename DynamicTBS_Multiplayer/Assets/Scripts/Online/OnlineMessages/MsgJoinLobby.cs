using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgJoinLobby : OnlineMessage
{
    public bool create; // Indicates whether a lobby should be created 
    public string lobbyName;
    public UserData userData;

    public MsgJoinLobby() // Constructing a message.
    {
        Code = OnlineMessageCode.JOIN_LOBBY;
    }

    public MsgJoinLobby(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.JOIN_LOBBY;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte(ToByte(create));
        writer.WriteFixedString32(lobbyName);
        userData.Serialize(ref writer);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        create = ToBool(reader.ReadByte());
        lobbyName = reader.ReadFixedString32().Value;
        userData = UserData.Deserialize(reader);
    }

    public override void ReceivedOnClient()
    {
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        if(create)
        {
            OnlineServer.Instance.CreateLobby(lobbyName, cnn, userData);
        } 
        else
        {
            OnlineServer.Instance.JoinLobby(new LobbyId(LobbyId, lobbyName), cnn, userData);
        }
    }
}
