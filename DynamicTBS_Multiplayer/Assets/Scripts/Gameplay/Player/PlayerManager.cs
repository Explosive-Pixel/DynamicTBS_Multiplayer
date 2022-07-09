using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Player currentPlayer;

    private Player bluePlayer;
    private Player pinkPlayer;

    public PlayerManager()
    {
        bluePlayer = new Player();
        pinkPlayer = new Player();

        currentPlayer = bluePlayer;
    }

    public Player GetOtherPlayer(Player player)
    {
        if (player == bluePlayer)
            return pinkPlayer;
        return bluePlayer;
    }

    public bool IsBluePlayer(Player player)
    {
        return player == bluePlayer;
    }
}