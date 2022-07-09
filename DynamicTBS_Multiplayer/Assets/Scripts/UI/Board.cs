using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private List<Tile> tiles;

    public Board()
    {
        CreateBoard();
    }

    private void CreateBoard() 
    {
        int boardSize = 9;
        for (int row = 0; row < boardSize; row++) {
            for (int column = 0; column < boardSize; column++) { 
                
            }
        }
    }
}