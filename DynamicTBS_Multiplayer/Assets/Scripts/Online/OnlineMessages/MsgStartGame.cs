using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgStartGame : OnlineMessage
{
    public float draftAndPlacementTime;
    public float gameplayTime;

    public MsgStartGame() // Constructing a message.
    {
        Code = OnlineMessageCode.START_GAME;
    }

    public MsgStartGame(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.START_GAME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFloat(draftAndPlacementTime);
        writer.WriteFloat(gameplayTime);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        draftAndPlacementTime = reader.ReadFloat();
        gameplayTime = reader.ReadFloat();
    }

    public override void ReceivedOnClient()
    {
        OnlineClient.Instance.StartGame();
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);

        OnlineServer.Instance.StartGame(LobbyId, draftAndPlacementTime, gameplayTime);

        base.ReceivedOnServer(cnn);
    }
}
