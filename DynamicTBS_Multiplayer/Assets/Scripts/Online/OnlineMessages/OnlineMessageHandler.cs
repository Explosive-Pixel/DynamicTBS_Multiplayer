using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public enum OnlineMessageCode
{
    METADATA = 0,
    JOIN_LOBBY = 1,
    WELCOME_CLIENT = 2,
    UPDATE_CLIENT = 3,
    UI_ACTION = 4,
    SERVER_NOTIFICATION = 5,
    DRAFT_CHARACTER = 6,
    PERFORM_ACTION = 7,
    SYNC_TIMER = 8,
    START_GAME = 9,
    UPDATE_SERVER = 10,
    GAME_OVER = 11
}

public static class OnlineMessageHandler
{
    public static void HandleData(DataStreamReader stream, NetworkConnection cnn, OnlineServer server = null) // Called when message received to read it.
    {
        OnlineMessage msg = null;
        var opCode = (OnlineMessageCode)stream.ReadByte(); // Reads first byte of message.

        switch (opCode) // Makes server and client able to decode messages when they're being received.
        {
            case OnlineMessageCode.METADATA:
                msg = new MsgMetadata(stream);
                break;
            case OnlineMessageCode.JOIN_LOBBY:
                msg = new MsgJoinLobby(stream);
                break;
            case OnlineMessageCode.WELCOME_CLIENT:
                msg = new MsgWelcomeClient(stream);
                break;
            case OnlineMessageCode.UPDATE_CLIENT:
                msg = new MsgUpdateClient(stream);
                break;
            case OnlineMessageCode.UI_ACTION:
                msg = new MsgUIAction(stream);
                break;
            case OnlineMessageCode.SERVER_NOTIFICATION:
                msg = new MsgServerNotification(stream);
                break;
            case OnlineMessageCode.DRAFT_CHARACTER:
                msg = new MsgDraftCharacter(stream);
                break;
            case OnlineMessageCode.PERFORM_ACTION:
                msg = new MsgPerformAction(stream);
                break;
            case OnlineMessageCode.SYNC_TIMER:
                msg = new MsgSyncTimer(stream);
                break;
            case OnlineMessageCode.START_GAME:
                msg = new MsgStartGame(stream);
                break;
            case OnlineMessageCode.UPDATE_SERVER:
                msg = new MsgUpdateServer(stream);
                break;
            case OnlineMessageCode.GAME_OVER:
                msg = new MsgGameOver(stream);
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
}
