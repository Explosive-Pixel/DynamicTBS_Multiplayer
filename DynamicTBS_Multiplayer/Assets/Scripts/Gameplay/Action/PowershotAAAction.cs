using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject attackCirclePrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> powershotTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return powershotTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new List<GameObject>();

    private void Awake()
    {
        GameplayEvents.OnGameplayPhaseStart += Register;
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

        if(tile.GetRow() > 0 && tile.GetRow() < Board.boardSize - 1 && tile.GetColumn() > 0 && tile.GetColumn() < Board.boardSize - 1)
            return 4;

        if ((tile.GetRow() == 0 || tile.GetRow() == Board.boardSize - 1) && (tile.GetColumn() == 0 || tile.GetColumn() == Board.boardSize - 1))
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
        Vector3 shooterPosition = characterTile.GetPosition();

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
            if (tile.GetCurrentInhabitant() != null && tile.GetCurrentInhabitant().isAttackableBy(characterInAction))
                tile.GetCurrentInhabitant().TakeDamage(PowershotAA.powershotDamage);
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

        List<GameObject> targetList = new List<GameObject>();

        for (int i = 0; i < Board.boardSize; i++)
        {
            if (i != characterTile.GetColumn())
            {
                targetList.Add(ActionUtils.InstantiateActionPosition(Board.FindPosition(characterTile.GetRow(), i), attackCirclePrefab));
            }

            if (i != characterTile.GetRow())
            {
                targetList.Add(ActionUtils.InstantiateActionPosition(Board.FindPosition(i, characterTile.GetColumn()), attackCirclePrefab));
            }
        }

        return targetList;
    }

    private void Register()
    {
        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameplayPhaseStart -= Register;
    }
}
