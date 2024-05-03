using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject attackCirclePrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> powershotTargets = new();
    public List<GameObject> ActionDestinations { get { return powershotTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        patternTargets = CreateDestinations(character);
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        Tile tile = Board.GetTileByCharacter(character);

        if (tile.Row > 0 && tile.Row < Board.Rows - 1 && tile.Column > 0 && tile.Column < Board.Columns - 1)
            return 4;

        if ((tile.Row == 0 || tile.Row == Board.Rows - 1) && (tile.Column == 0 || tile.Column == Board.Columns - 1))
            return 2;

        return 3;
    }

    public void CreateActionDestinations(Character character)
    {
        powershotTargets = CreateDestinations(character);
        characterInAction = character;
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Vector3 clickPosition = actionDestination.transform.position;
        Tile characterTile = Board.GetTileByCharacter(characterInAction);
        Vector3 shooterPosition = characterTile.gameObject.transform.position;

        Vector3 shootDirection = Vector3.up;
        if (clickPosition.y == shooterPosition.y)
        {
            if (clickPosition.x < shooterPosition.x)
                shootDirection = Vector3.left;
            else
                shootDirection = Vector3.right;
        }
        else
        {
            if (clickPosition.y < shooterPosition.y)
            {
                shootDirection = Vector3.down;
            }
        }

        List<Tile> hitCharacterTiles = Board.GetAllOccupiedTilesInOneDirection(characterTile, shootDirection);

        foreach (Tile tile in hitCharacterTiles)
        {
            if (tile.CurrentInhabitant != null && tile.CurrentInhabitant.isAttackableBy(characterInAction))
                tile.CurrentInhabitant.TakeDamage(PowershotAA.damage);
        }
        characterInAction.TakeDamage(PowershotAA.selfDamage);

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(powershotTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<GameObject> CreateDestinations(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        List<GameObject> targetList = new();

        for (int i = 0; i < Board.Columns; i++)
        {
            if (i != characterTile.Column)
            {
                targetList.Add(ActionUtils.InstantiateActionPosition(this, Board.GetTileByCoordinates(characterTile.Row, i).gameObject.transform.position, attackCirclePrefab));
            }
        }

        for (int i = 0; i < Board.Rows; i++)
        {
            if (i != characterTile.Row)
            {
                targetList.Add(ActionUtils.InstantiateActionPosition(this, Board.GetTileByCoordinates(i, characterTile.Column).gameObject.transform.position, attackCirclePrefab));
            }
        }

        return targetList;
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
