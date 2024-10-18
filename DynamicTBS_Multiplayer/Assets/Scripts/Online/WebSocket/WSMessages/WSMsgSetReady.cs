using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMsgSetReady : WSMessage
{
    public WSMsgSetReady()
    {
        code = WSMessageCode.WSMsgSetReadyCode;
    }
}
