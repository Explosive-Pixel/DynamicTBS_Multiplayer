using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLayouts : MonoBehaviour
{
    [SerializeField] private TileMap[] boardDesigns;

    public TileMap GetBoardLayout(int i)
    {
        return boardDesigns[i];
    }
}