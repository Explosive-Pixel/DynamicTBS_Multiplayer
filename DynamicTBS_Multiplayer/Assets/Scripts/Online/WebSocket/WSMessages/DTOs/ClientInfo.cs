using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClientInfo
{
    public string uuid;
    public string name;
    public bool isPlayer;
    public PlayerType side;
    public bool isReady;
}
