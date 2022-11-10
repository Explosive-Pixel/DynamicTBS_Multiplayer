using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public enum OperationCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    DRAFT_CHARACTER = 4,
    PERFORM_ACTION = 5,
    REMATCH = 6
}

public static class NetUtility
{
    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null) // Called when message received to read it.
    {
        NetMessage msg = null;
        var opCode = (OperationCode)stream.ReadByte(); // Reads first byte of message.

        switch (opCode) // Makes server and client able to decode messages when they're being received.
        {
            case OperationCode.KEEP_ALIVE:
                msg = new NetKeepAlive(stream);
                break;
            case OperationCode.WELCOME:
                msg = new NetWelcome(stream);
                break;
            case OperationCode.START_GAME:
                msg = new NetStartGame(stream);
                break;
            case OperationCode.DRAFT_CHARACTER:
                msg = new NetDraftCharacter(stream);
                break;
            case OperationCode.PERFORM_ACTION:
                msg = new NetPerformAction(stream);
                break;
            case OperationCode.REMATCH:
                msg = new NetRematch(stream);
                break;
            default:
                Debug.LogError("Message received had no operation code.");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();
    }

    #region NetMessages

    // Client side messages.
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_DRAFT_CHARACTER;
    public static Action<NetMessage> C_PERFORM_ACTION;
    public static Action<NetMessage> C_REMATCH;

    // Server side messages.
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_DRAFT_CHARACTER;
    public static Action<NetMessage, NetworkConnection> S_PERFORM_ACTION;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;

    #endregion
}