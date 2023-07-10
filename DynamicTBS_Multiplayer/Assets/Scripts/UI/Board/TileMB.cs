using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileMB : MonoBehaviour
{
    public int row;
    public int column;

    public GameObject machineCore;
    public GameObject unitStart_pink;
    public GameObject unitStart_blue;
    public GameObject captainStart_pink;
    public GameObject captainStart_blue;
    public GameObject electrifyMarker;

    private GameObject UnitStart(PlayerType side) { return side == PlayerType.blue ? unitStart_blue : unitStart_pink; }
    private GameObject CaptainStart(PlayerType side) { return side == PlayerType.blue ? captainStart_blue : captainStart_pink; }

    private TileType tileType;
    public TileType TileType { get { return tileType; } }
    private PlayerType side;
    public PlayerType Side { get { return side; } }

    private Character currentInhabitant;
    public Character CurrentInhabitant { get { return currentInhabitant; } set { currentInhabitant = value; } }

    private State state;

    public delegate bool IsChangeable();
    public IsChangeable isChangeable;

    public void Init(TileType tileType, PlayerType side)
    {
        this.side = side;
        this.currentInhabitant = null;
        this.state = null;

        Transform(tileType);
        SubscribeEvents();
    }

    public void Transform(TileType tileType)
    {
        this.tileType = tileType;

        gameObject.GetComponent<Image>().enabled = tileType != TileType.EmptyTile;
        machineCore.gameObject.SetActive(tileType == TileType.GoalTile);
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
        return currentInhabitant != null;
    }

    public bool IsAccessible()
    {
        return !IsOccupied() && !IsHole();
    }

    private void UpdateCurrentInhabitant(ActionMetadata actionMetadata)
    {
        if (actionMetadata.ExecutedActionType != ActionType.Move || actionMetadata.ActionDestinationPosition == null)
            return;

        if(UIUtils.HasSamePosition(gameObject, actionMetadata.ActionDestinationPosition.Value))
        {
            currentInhabitant = actionMetadata.CharacterInAction;
        } else if(currentInhabitant == actionMetadata.CharacterInAction)
        {
            currentInhabitant = null;
        }
    }

    private void UpdateCurrentInhabitantAfterCharacterDeath(Character character, Vector3 position)
    {
        if (currentInhabitant == character)
            currentInhabitant = null;
    }

    private void TransformToFloorTile(Character character)
    {
        if (currentInhabitant == character)
            Transform(TileType.FloorTile);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnFinishAction += UpdateCurrentInhabitant;
        PlacementEvents.OnPlaceCharacter += TransformToFloorTile;
        CharacterEvents.OnCharacterDeath += UpdateCurrentInhabitantAfterCharacterDeath;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnFinishAction -= UpdateCurrentInhabitant;
        PlacementEvents.OnPlaceCharacter -= TransformToFloorTile;
        CharacterEvents.OnCharacterDeath -= UpdateCurrentInhabitantAfterCharacterDeath;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
