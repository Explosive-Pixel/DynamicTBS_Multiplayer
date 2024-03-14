using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgUpdateServer : WSMessage
{
    public GamePhase gamePhase;
    public PlayerType currentPlayer;

    public WSMsgUpdateServer()
    {
        code = WSMessageCode.WSMsgUpdateServerCode;
    }
}
