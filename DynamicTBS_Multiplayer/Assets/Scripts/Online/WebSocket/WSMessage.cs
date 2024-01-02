using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WSMessageCode
{
    WSMsgMetadataCode = 0,
    WSMsgJoinLobbyCode = 1,
    WSMsgWelcomeClientCode = 2,
    WSMsgGameHistoryCode = 3,
    WSMsgStartGameCode = 4,
    WSMsgPauseGameCode = 5,
    WSMsgServerNotificationCode = 6,
    WSMsgUpdateServerCode = 7,
    WSMsgUpdateClientCode = 8,
    WSMsgGameOverCode = 9,
    WSMsgDraftCharacterCode = 10,
    WSMsgPerformActionCode = 11,
    WSMsgUIActionCode = 12
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
