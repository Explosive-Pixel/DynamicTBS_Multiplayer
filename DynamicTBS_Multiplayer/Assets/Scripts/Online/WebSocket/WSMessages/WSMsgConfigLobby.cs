using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMsgConfigLobby : WSMessage
{
    public ClientInfo clientInfo;
    public GameConfig gameConfig;

    public WSMsgConfigLobby()
    {
        code = WSMessageCode.WSMsgConfigLobbyCode;
    }
}
