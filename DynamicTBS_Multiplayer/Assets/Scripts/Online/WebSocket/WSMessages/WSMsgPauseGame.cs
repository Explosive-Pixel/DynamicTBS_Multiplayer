using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgPauseGame : WSMessage
{
    public bool pause;
    public WSMsgUpdateClient timerUpdate;

    public WSMsgPauseGame()
    {
        code = WSMessageCode.WSMsgPauseGameCode;
    }

    public override void HandleMessage()
    {
        GameplayEvents.PauseGame(pause);

        //if (!pause)
        GameplayEvents.PauseGameOnline(this);
    }
}
