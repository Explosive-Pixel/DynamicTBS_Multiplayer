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
        LobbyId = reader.ReadInt();
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

        OnlineServer.Instance.StartGame(LobbyId, timerSetup, selectedMap);
    }
}
