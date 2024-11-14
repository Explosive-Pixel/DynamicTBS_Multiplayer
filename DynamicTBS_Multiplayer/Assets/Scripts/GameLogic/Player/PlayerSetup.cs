using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    private static string playerName;
    //public static string Name { get { return playerName ?? GetName(Side); } private set { playerName = value; } }
    public static string Name { get { return playerName; } private set { playerName = value; } }
    public static PlayerType Side { get; private set; } = PlayerType.none;

    public static bool SetupCompleted { get { return Name != null && Side != PlayerType.none; } }
    public static bool IsPlayer { get { return Side != PlayerType.none; } }

    public static void Setup(ClientInfo clientInfo)
    {
        SetupName(clientInfo.name);
        SetupSide(clientInfo.side);
    }

    public static void SetupName(string name)
    {
        Name = name;
    }

    public static void SetupSide(PlayerType side)
    {
        Side = side;
    }

    public static bool IsSide(PlayerType side)
    {
        return IsPlayer && Side == side;
    }

    public static bool IsNotSide(PlayerType side)
    {
        return IsPlayer && Side != side;
    }

    private static string GetName(PlayerType side)
    {
        if (side == PlayerType.none)
            return null;

        return side == PlayerType.pink ? "Pink" : "Blue";
    }

    private void Reset()
    {
        Name = null;
        Side = PlayerType.none;
    }
}
