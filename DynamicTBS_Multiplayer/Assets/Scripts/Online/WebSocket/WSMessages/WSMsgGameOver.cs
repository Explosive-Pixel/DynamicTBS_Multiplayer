using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgGameOver : WSMessage
{
    public WSMsgGameOver()
    {
        code = WSMessageCode.WSMsgGameOverCode;
    }
}
