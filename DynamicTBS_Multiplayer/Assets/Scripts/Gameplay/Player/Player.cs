using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private PlayerType type;

    public Player(PlayerType type) 
    {
        this.type = type;
    }

    public PlayerType GetPlayerType() 
    {
        return this.type;
    }
}