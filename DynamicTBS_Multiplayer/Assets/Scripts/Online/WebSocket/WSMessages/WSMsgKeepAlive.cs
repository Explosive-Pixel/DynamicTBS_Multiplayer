using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WSMsgKeepAlive : WSMessage
{
    public WSMsgKeepAlive()
    {
        code = WSMessageCode.WSMsgKeepAliveCode;
    }
}
