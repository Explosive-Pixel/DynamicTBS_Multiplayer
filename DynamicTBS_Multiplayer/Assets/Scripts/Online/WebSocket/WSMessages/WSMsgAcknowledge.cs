using System;

[Serializable]
public class WSMsgAcknowledge : WSMessage
{
    public string messageUuid;

    public WSMsgAcknowledge()
    {
        code = WSMessageCode.WSMsgAcknowledgeCode;
    }
}
