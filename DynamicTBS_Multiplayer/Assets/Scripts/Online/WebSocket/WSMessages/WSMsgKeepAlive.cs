using System;

[Serializable]
public class WSMsgKeepAlive : WSMessage
{
    public WSMsgKeepAlive()
    {
        code = WSMessageCode.WSMsgKeepAliveCode;
    }
}
