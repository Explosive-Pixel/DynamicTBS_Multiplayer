using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgStartGame : OnlineMessage
{
    public TimerSetupType timerSetup;
    public MapType selectedMap;

    public MsgStartGame() // Constructing a message.
    {
        Code = OnlineMessageCode.START_GAME;
    }

    public MsgStartGame(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.START_GAME;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte((byte)timerSetup);
        writer.WriteByte((byte)selectedMap);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        timerSetup = (TimerSetupType)reader.ReadByte();
        selectedMap = (MapType)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        OnlineClient.Instance.StartGame(timerSetup, selectedMap);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);
        Debug.Log("Received Start Game msg: timerSetup " + timerSetup + ", selectedMap " + selectedMap);
        OnlineServer.Instance.StartGame(LobbyId, timerSetup, selectedMap);
    }
}
