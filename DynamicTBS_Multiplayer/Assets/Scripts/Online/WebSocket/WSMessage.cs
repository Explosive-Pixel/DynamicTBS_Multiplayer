using System;
using UnityEngine;

public enum WSMessageCode
{
    WSMsgAcknowledgeCode = 0,
    WSMsgKeepAliveCode = 1,
    WSMsgWelcomeClientCode = 2,

    WSMsgLobbyListCode = 3,
    WSMsgCreateLobbyCode = 4,
    WSMsgJoinLobbyCode = 5,
    WSMsgLobbyInfoCode = 6,
    WSMsgSetReadyCode = 7,
    WSMsgConfigLobbyCode = 8,
    WSMsgCloseLobbyCode = 9,

    WSMsgGameHistoryCode = 10,
    WSMsgStartGameCode = 11,
    WSMsgPauseGameCode = 12,
    WSMsgServerNotificationCode = 13,
    WSMsgUpdateServerCode = 14,
    WSMsgUpdateClientCode = 15,
    WSMsgGameOverCode = 16,
    WSMsgDraftCharacterCode = 17,
    WSMsgPerformActionCode = 18,
    WSMsgUIActionCode = 19
}

[Serializable]
public abstract class WSMessage
{
    public WSMessageCode code;
    public string uuid = Guid.NewGuid().ToString();
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
            case WSMessageCode.WSMsgAcknowledgeCode:
                return Deserialize<WSMsgAcknowledge>(json);
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
            case WSMessageCode.WSMsgCloseLobbyCode:
                return Deserialize<WSMsgCloseLobby>(json);
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

    public static bool Record(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgCloseLobbyCode:
            case WSMessageCode.WSMsgStartGameCode:
            case WSMessageCode.WSMsgPauseGameCode:
            case WSMessageCode.WSMsgServerNotificationCode:
            case WSMessageCode.WSMsgUpdateClientCode:
            case WSMessageCode.WSMsgDraftCharacterCode:
            case WSMessageCode.WSMsgPerformActionCode:
            case WSMessageCode.WSMsgUIActionCode:
                return true;
            default:
                return false;
        }
    }

    private static T Deserialize<T>(string json) where T : WSMessage
    {
        return JsonUtility.FromJson<T>(json);
    }

    public virtual void HandleMessage() { }

    public override string ToString()
    {
        return $"{GetType().Name} (UUID={uuid})";
    }
}
