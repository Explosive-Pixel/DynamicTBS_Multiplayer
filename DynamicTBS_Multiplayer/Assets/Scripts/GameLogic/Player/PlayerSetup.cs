using System;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    private static string localizedPink = "Pink";
    private static string localizedBlue = "Blue";
    private static readonly int maxPlayerNameCharacters = 16;

    private static string playerName;
    public static string Name { get { return playerName; } private set { playerName = value; } }
    public static PlayerType Side { get; private set; } = PlayerType.none;
    public static string SideName { get { return GetSideName(Side); } }

    public static bool SetupCompleted { get { return Name != null && Side != PlayerType.none; } }
    public static bool IsPlayer { get { return Side != PlayerType.none; } }

    public static void Setup(ClientInfo clientInfo)
    {
        SetupName(clientInfo.name);
        SetupSide(clientInfo.side);
    }

    public static void SetupName(string name)
    {
        if (name == null)
            Name = null;
        else
        {
            string trimmed = name.Trim();
            Name = trimmed[..Math.Min(maxPlayerNameCharacters, trimmed.Length)];
        }
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

    public static string GetSideName(PlayerType side)
    {
        if (side == PlayerType.none)
            return null;

        return side == PlayerType.pink ? localizedPink : localizedBlue;
    }

    public static void SetLocalizedDefault(PlayerType side, string value)
    {
        if (side == PlayerType.pink)
            localizedPink = value;
        else if (side == PlayerType.blue)
            localizedBlue = value;
    }

    private void Reset()
    {
        Name = null;
        Side = PlayerType.none;
    }
}
