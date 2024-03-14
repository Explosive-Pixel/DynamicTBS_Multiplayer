using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgMetadata : WSMessage
{
    public int playerCount;
    public int spectatorCount;
    public string pinkName;
    public string blueName;

    public WSMsgMetadata()
    {
        code = WSMessageCode.WSMsgMetadataCode;
    }
}
