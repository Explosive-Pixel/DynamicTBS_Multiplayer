using System;
using UnityEngine;

public enum WSMessageCode
{
    WSMsgKeepAliveCode = 0,
    WSMsgWelcomeClientCode = 1,

    WSMsgLobbyListCode = 2,
    WSMsgCreateLobbyCode = 3,
    WSMsgJoinLobbyCode = 4,
    WSMsgLobbyInfoCode = 5,
    WSMsgSetReadyCode = 6,
    WSMsgConfigLobbyCode = 7,

    WSMsgGameHistoryCode = 8,
    WSMsgStartGameCode = 9,
    WSMsgPauseGameCode = 10,
    WSMsgServerNotificationCode = 11,
    WSMsgUpdateServerCode = 12,
    WSMsgUpdateClientCode = 13,
    WSMsgGameOverCode = 14,
    WSMsgDraftCharacterCode = 15,
    WSMsgPerformActionCode = 16,
    WSMsgUIActionCode = 17,
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
            case WSMessageCode.WSMsgLobbyListCode:
                return Deserialize<WSMsgLobbyList>(json);
            case WSMessageCode.WSMsgJoinLobbyCode:
                return Deserialize<WSMsgJoinLobby>(json);
            case WSMessageCode.WSMsgWelcomeClientCode:
                return Deserialize<WSMsgWelcomeClient>(json);
            case WSMessageCode.WSMsgGameHistoryCode:
                return Deserialize<WSMsgGameHistory>(json);
            case WSMessageCode.WSMsgSetReadyCode:
                return Deserialize<WSMsgSetReady>(json);
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
            case WSMessageCode.WSMsgConfigLobbyCode:
                return Deserialize<WSMsgConfigLobby>(json);
        }

        return null;
    }

    private static T Deserialize<T>(string json) where T : WSMessage
    {
        return JsonUtility.FromJson<T>(json);
    }

    public virtual void HandleMessage() { }
}
