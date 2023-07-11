using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileMB : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int column;

    public GameObject machineCore;
    public GameObject unitStart_pink;
    public GameObject unitStart_blue;
    public GameObject captainStart_pink;
    public GameObject captainStart_blue;

    public int Row { get { return row; } }
    public int Column { get { return column; } }

    private GameObject UnitStart(PlayerType side) { return side == PlayerType.blue ? unitStart_blue : unitStart_pink; }
    private GameObject CaptainStart(PlayerType side) { return side == PlayerType.blue ? captainStart_blue : captainStart_pink; }

    private TileType tileType;
    public TileType TileType { get { return tileType; } }
    private PlayerType side;
    public PlayerType Side { get { return side; } }

    public CharacterMB CurrentInhabitant { get { return gameObject.GetComponentInChildren<CharacterMB>(); } }

    private State state;

    public delegate bool IsChangeable();
    public IsChangeable isChangeable;

    public void Init(TileType tileType, PlayerType side)
    {
        this.side = side;
        this.state = null;

        Transform(tileType);
        SubscribeEvents();
    }

    public void Transform(TileType tileType)
    {
        this.tileType = tileType;

        gameObject.GetComponent<SpriteRenderer>().enabled = tileType != TileType.EmptyTile;
        machineCore.SetActive(tileType == TileType.GoalTile);
        UnitStart(side).SetActive(tileType == TileType.StartTile);
        UnitStart(PlayerManager.GetOtherSide(side)).SetActive(false);
        CaptainStart(side).SetActive(tileType == TileType.MasterStartTile);
        CaptainStart(PlayerManager.GetOtherSide(side)).SetActive(false);

        this.isChangeable = () => !IsGoal();
    }

    public bool IsElectrified()
    {
        return state != null && state.GetType() == typeof(ElectrifyAA) && state.IsActive();
    }

    public void SetState(TileStateType stateType)
    {
        this.state = TileStateFactory.Create(stateType, gameObject);
    }

    private void ResetState()
    {
        if (state != null)
            state.Destroy();

        state = null;
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

    private void TransformToFloorTile(CharacterMB character)
    {
        if (CurrentInhabitant == character)
            Transform(TileType.FloorTile);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        //PlacementEvents.OnPlaceCharacter += TransformToFloorTile;
    }

    private void UnsubscribeEvents()
    {
        //PlacementEvents.OnPlaceCharacter -= TransformToFloorTile;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
