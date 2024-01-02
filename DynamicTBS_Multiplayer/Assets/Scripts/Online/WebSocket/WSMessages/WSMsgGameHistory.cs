using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class WSMsgGameHistory : WSMessage
{
    public string[] msgHistory;

    public WSMsgGameHistory()
    {
        code = WSMessageCode.WSMsgGameHistoryCode;
    }

    public List<WSMessage> Deserialize()
    {
        return msgHistory.ToList().ConvertAll(msg => Deserialize(msg));
    }

    public override void HandleMessage()
    {
        Client.ToggleIsLoadingGame();
        Client.Ws.LoadGame(Deserialize());
    }
}
