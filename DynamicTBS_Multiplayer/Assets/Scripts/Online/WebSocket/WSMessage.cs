using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WSMessageCode
{
    WSMsgKeepAliveCode = 0,
    WSMsgMetadataCode = 1,
    WSMsgLobbyListCode = 2,
    WSMsgJoinLobbyCode = 3,
    WSMsgWelcomeClientCode = 4,
    WSMsgGameHistoryCode = 5,
    WSMsgStartGameCode = 6,
    WSMsgPauseGameCode = 7,
    WSMsgServerNotificationCode = 8,
    WSMsgUpdateServerCode = 9,
    WSMsgUpdateClientCode = 10,
    WSMsgGameOverCode = 11,
    WSMsgDraftCharacterCode = 12,
    WSMsgPerformActionCode = 13,
    WSMsgUIActionCode = 14,

    WSMsgCreateLobbyCode = 15,
    WSMsgLobbyInfoCode = 16
}

[Serializable]
public abstract class WSMessage
{
    public WSMessageCode code;
    public int lobbyId;

    public string Serialize()
    {
        return (int)code + "$" + JsonUtility.ToJson(this);
    }

    public static WSMessage Deserialize(string msg)
    {
        string[] msgArr = msg.Split("$");
        WSMessageCode code = (WSMessageCode)int.Parse(msgArr[0]);
        string json = msg[(msg.IndexOf('$') + 1)..];

        switch (code)
        {
            case WSMessageCode.WSMsgKeepAliveCode:
                return Deserialize<WSMsgKeepAlive>(json);
            case WSMessageCode.WSMsgMetadataCode:
                return Deserialize<WSMsgMetadata>(json);
            case WSMessageCode.WSMsgLobbyListCode:
                return Deserialize<WSMsgLobbyList>(json);
            case WSMessageCode.WSMsgJoinLobbyCode:
                return Deserialize<WSMsgJoinLobby>(json);
            case WSMessageCode.WSMsgWelcomeClientCode:
                return Deserialize<WSMsgWelcomeClient>(json);
            case WSMessageCode.WSMsgGameHistoryCode:
                return Deserialize<WSMsgGameHistory>(json);
            case WSMessageCode.WSMsgStartGameCode:
                return Deserialize<WSMsgStartGame>(json);
            case WSMessageCode.WSMsgPauseGameCode:
                return Deserialize<WSMsgPauseGame>(json);
            case WSMessageCode.WSMsgServerNotificationCode:
                return Deserialize<WSMsgServerNotification>(json);
            case WSMessageCode.WSMsgUpdateServerCode:
                return Deserialize<WSMsgUpdateServer>(json);
            case WSMessageCode.WSMsgUpdateClientCode:
                return Deserialize<WSMsgUpdateClient>(json);
            case WSMessageCode.WSMsgGameOverCode:
                return Deserialize<WSMsgGameOver>(json);
            case WSMessageCode.WSMsgDraftCharacterCode:
                return Deserialize<WSMsgDraftCharacter>(json);
            case WSMessageCode.WSMsgPerformActionCode:
                return Deserialize<WSMsgPerformAction>(json);
            case WSMessageCode.WSMsgUIActionCode:
                return Deserialize<WSMsgUIAction>(json);
            case WSMessageCode.WSMsgCreateLobbyCode:
                return Deserialize<WSMsgCreateLobby>(json);
            case WSMessageCode.WSMsgLobbyInfoCode:
                return Deserialize<WSMsgLobbyInfo>(json);
        }

        return null;
    }

    private static T Deserialize<T>(string json) where T : WSMessage
    {
        return JsonUtility.FromJson<T>(json);
    }

    public virtual void HandleMessage() { }
}
