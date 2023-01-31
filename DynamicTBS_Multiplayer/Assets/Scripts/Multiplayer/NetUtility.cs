using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public enum OperationCode
{
    CHANGE_LOAD_GAME_STATUS = 1,
    WELCOME = 2,
    START_GAME = 3,
    DRAFT_CHARACTER = 4,
    PERFORM_ACTION = 5,
    EXECUTE_UIACTION = 6,
    CONNECTION_FORBIDDEN = 7,
    METADATA = 8,
    UPDATE_TIMER = 9,
    EXECUTE_SERVER_ACTION = 10
}

public static class NetUtility
{
    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null) // Called when message received to read it.
    {
        NetMessage msg = null;
        var opCode = (OperationCode)stream.ReadByte(); // Reads first byte of message.

        switch (opCode) // Makes server and client able to decode messages when they're being received.
        {
            case OperationCode.CHANGE_LOAD_GAME_STATUS:
                // Debug.Log("Client: Reading message CHANGE_LOAD_GAME_STATUS");
                msg = new NetChangeLoadGameStatus(stream);
                break;
            case OperationCode.WELCOME:
                // Debug.Log("Client: Reading message WELCOME");
                msg = new NetWelcome(stream);
                break;
            case OperationCode.START_GAME:
                // Debug.Log("Client: Reading message START_GAME");
                msg = new NetStartGame(stream);
                break;
            case OperationCode.DRAFT_CHARACTER:
                // Debug.Log("Client: Reading message DRAFT_CHARACTER");
                msg = new NetDraftCharacter(stream);
                break;
            case OperationCode.PERFORM_ACTION:
                // Debug.Log("Client: Reading message PERFORM_ACTION");
                msg = new NetPerformAction(stream);
                break;
            case OperationCode.EXECUTE_UIACTION:
                // Debug.Log("Client: Reading message EXECUTE_UIACTION");
                msg = new NetExecuteUIAction(stream);
                break;
            case OperationCode.CONNECTION_FORBIDDEN:
                // Debug.Log("Client: Reading message CONNECTION_FORBIDDEN");
                msg = new NetConnectionForbidden(stream);
                break;
            case OperationCode.METADATA:
                // Debug.Log("Client: Reading message METADATA");
                msg = new NetMetadata(stream);
                break;
            case OperationCode.UPDATE_TIMER:
                // Debug.Log("Client: Reading message UPDATE_TIMER");
                msg = new NetUpdateTimer(stream);
                break;
            case OperationCode.EXECUTE_SERVER_ACTION:
                // Debug.Log("Client: Reading message EXECUTE_SERVER_ACTION");
                msg = new NetExecuteServerAction(stream);
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
    public static Action<NetMessage> C_CHANGE_LOAD_GAME_STATUS;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_DRAFT_CHARACTER;
    public static Action<NetMessage> C_PERFORM_ACTION;
    public static Action<NetMessage> C_EXECUTE_UIACTION;
    public static Action<NetMessage> C_CONNECTION_FORBIDDEN;
    public static Action<NetMessage> C_METADATA;
    public static Action<NetMessage> C_UPDATE_TIMER;
    public static Action<NetMessage> C_EXECUTE_SERVER_ACTION;

    // Server side messages.
    public static Action<NetMessage, NetworkConnection> S_CHANGE_LOAD_GAME_STATUS;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_DRAFT_CHARACTER;
    public static Action<NetMessage, NetworkConnection> S_PERFORM_ACTION;
    public static Action<NetMessage, NetworkConnection> S_EXECUTE_UIACTION;
    public static Action<NetMessage, NetworkConnection> S_CONNECTION_FORBIDDEN;
    public static Action<NetMessage, NetworkConnection> S_METADATA;
    public static Action<NetMessage, NetworkConnection> S_UPDATE_TIMER;
    public static Action<NetMessage, NetworkConnection> S_EXECUTE_SERVER_ACTION;

    #endregion
}