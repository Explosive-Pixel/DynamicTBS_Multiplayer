using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Tile : MonoBehaviour
{
    public GameObject machineCore;
    public GameObject unitStart_pink;
    public GameObject unitStart_blue;
    public GameObject masterStart_pink;
    public GameObject masterStart_blue;

    private int row;
    private int column;

    public int Row { get { return row; } }
    public int Column { get { return column; } }
    public string Name { get { return gameObject.name; } }

    private GameObject UnitStart(PlayerType side) { return side == PlayerType.blue ? unitStart_blue : unitStart_pink; }
    private GameObject MasterStart(PlayerType side) { return side == PlayerType.blue ? masterStart_blue : masterStart_pink; }

    private TileType tileType;
    public TileType TileType { get { return tileType; } }
    private PlayerType side;
    public PlayerType Side { get { return side; } }

    public Character CurrentInhabitant { get { return gameObject.GetComponentInChildren<Character>(); } }

    public delegate bool IsChangeable();
    public IsChangeable isChangeable = () => true;

    public void Init(TileType tileType, PlayerType side)
    {
        this.side = side;

        row = int.Parse(Name[1].ToString()) - 1;
        column = ((int)Name[0]) - 65;

        Transform(tileType);
        isChangeable = () => !IsGoal();

        SubscribeEvents();
    }

    public void Transform(TileType tileType)
    {
        if (!isChangeable())
            return;

        this.tileType = tileType;

        if (IsHole())
            ResetStates();

        gameObject.GetComponent<SpriteRenderer>().enabled = tileType != TileType.EmptyTile;
        machineCore.SetActive(tileType == TileType.GoalTile);
        UnitStart(side).SetActive(tileType == TileType.StartTile);
        UnitStart(PlayerManager.GetOtherSide(side)).SetActive(false);
        MasterStart(side).SetActive(tileType == TileType.MasterStartTile);
        MasterStart(PlayerManager.GetOtherSide(side)).SetActive(false);
    }

    private void ResetStates()
    {
        gameObject.GetComponents<IState>().ToList().ForEach(state => state.Destroy());
    }

    public bool IsGoal()
    {
        return tileType == TileType.GoalTile;
    }

    public bool IsHole()
    {
        return tileType == TileType.EmptyTile;
    }

    public bool IsNormalFloor()
    {
        return !IsHole() && !IsGoal();
    }

    public bool IsOccupied()
    {
        return CurrentInhabitant != null;
    }

    public bool IsAccessible()
    {
        return !IsOccupied() && !IsHole();
    }

    private void TransformToFloorTile(Character character)
    {
        if (CurrentInhabitant == character)
            Transform(TileType.FloorTile);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        PlacementEvents.OnPlaceCharacter += TransformToFloorTile;
    }

    private void UnsubscribeEvents()
    {
        PlacementEvents.OnPlaceCharacter -= TransformToFloorTile;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
