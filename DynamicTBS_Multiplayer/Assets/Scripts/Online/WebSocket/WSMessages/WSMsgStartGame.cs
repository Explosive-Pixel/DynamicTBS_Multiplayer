using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgStartGame : WSMessage
{
    public float draftAndPlacementTimeInSeconds;
    public float gameplayTimeInSeconds;
    public MapType mapType;
    public PlayerType adminSide;

    public WSMsgStartGame()
    {
        code = WSMessageCode.WSMsgStartGameCode;
    }

    public override void HandleMessage()
    {
        Client.StartGame(draftAndPlacementTimeInSeconds, gameplayTimeInSeconds, mapType, adminSide);
    }
}
