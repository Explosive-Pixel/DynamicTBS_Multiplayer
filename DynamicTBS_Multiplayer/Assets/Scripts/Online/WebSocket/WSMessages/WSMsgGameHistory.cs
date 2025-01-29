using System;
using System.Collections.Generic;
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
        WSClient.Instance.LoadGame(Deserialize());
    }
}
