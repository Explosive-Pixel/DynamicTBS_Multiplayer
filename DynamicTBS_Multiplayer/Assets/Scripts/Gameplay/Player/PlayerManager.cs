using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Player currentPlayer;

    private Player bluePlayer;
    private Player pinkPlayer;

    private void Awake()
    {
        bluePlayer = new Player(PlayerType.blue);
        pinkPlayer = new Player(PlayerType.pink);

        currentPlayer = pinkPlayer;
    }

    public Player GetOtherPlayer(Player player)
    {
        if (player == bluePlayer)
            return pinkPlayer;
        return bluePlayer;
    }

    public void NextPlayer() 
    {
        currentPlayer = GetOtherPlayer(currentPlayer);
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsBlueTurn()
    {
        if (currentPlayer == bluePlayer)
            return true;
        return false;
    }

    public bool IsCurrentPlayer(string name)
    {
        if (name.Contains("Blue") && currentPlayer == bluePlayer)
            return true;
        if (name.Contains("Pink") && currentPlayer == pinkPlayer)
            return true;
        return false;
    }
}