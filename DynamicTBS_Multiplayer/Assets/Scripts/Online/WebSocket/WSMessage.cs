using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WSMessageCode
{
    WSMsgKeepAliveCode = 0,
    WSMsgMetadataCode = 1,
    WSMsgJoinLobbyCode = 2,
    WSMsgWelcomeClientCode = 3,
    WSMsgGameHistoryCode = 4,
    WSMsgStartGameCode = 5,
    WSMsgPauseGameCode = 6,
    WSMsgServerNotificationCode = 7,
    WSMsgUpdateServerCode = 8,
    WSMsgUpdateClientCode = 9,
    WSMsgGameOverCode = 10,
    WSMsgDraftCharacterCode = 11,
    WSMsgPerformActionCode = 12,
    WSMsgUIActionCode = 13
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
        }

        return null;
    }

    private static T Deserialize<T>(string json) where T : WSMessage
    {
        return JsonUtility.FromJson<T>(json);
    }

    public virtual void HandleMessage() { }
}
