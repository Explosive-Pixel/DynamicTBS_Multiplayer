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
        return currentPlayer == bluePlayer;
    }

    public bool IsCurrentPlayer(string name)
    {
        if (name.ToLower().Contains(PlayerType.blue.ToString()) && IsBlueTurn())
            return true;
        if (name.ToLower().Contains(PlayerType.pink.ToString()) && !IsBlueTurn())
            return true;
        return false;
    }

    public Player GetPlayer(PlayerType playerType)
    {
        if (playerType == PlayerType.blue)
            return bluePlayer;
        return pinkPlayer;
    }
}