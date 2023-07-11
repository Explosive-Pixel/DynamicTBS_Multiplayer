using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject attackCirclePrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> powershotTargets = new();
    public List<GameObject> ActionDestinations { get { return powershotTargets; } }

    private CharacterMB characterInAction = null;
    public CharacterMB CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(CharacterMB character)
    {
        patternTargets = CreateDestinations(character);
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(CharacterMB character)
    {
        TileMB tile = BoardNew.GetTileByCharacter(character);

        if (tile.Row > 0 && tile.Row < BoardNew.Rows && tile.Column > 0 && tile.Column < BoardNew.Columns)
            return 4;

        if ((tile.Row == 0 || tile.Row == BoardNew.Rows) && (tile.Column == 0 || tile.Column == BoardNew.Columns))
            return 2;

        return 3;
    }

    public void CreateActionDestinations(CharacterMB character)
    {
        powershotTargets = CreateDestinations(character);
        characterInAction = character;
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Vector3 clickPosition = actionDestination.transform.position;
        TileMB characterTile = BoardNew.GetTileByCharacter(characterInAction);
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

        List<TileMB> hitCharacterTiles = BoardNew.GetAllOccupiedTilesInOneDirection(characterTile, shootDirection);

        foreach (TileMB tile in hitCharacterTiles)
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

    private List<GameObject> CreateDestinations(CharacterMB character)
    {
        TileMB characterTile = BoardNew.GetTileByCharacter(character);

        List<GameObject> targetList = new();

        for (int i = 0; i < BoardNew.Columns; i++)
        {
            if (i != characterTile.Column)
            {
                targetList.Add(ActionUtils.InstantiateActionPosition(BoardNew.GetTileByCoordinates(characterTile.Row, i).gameObject.transform.position, attackCirclePrefab));
            }
        }

        for (int i = 0; i < BoardNew.Rows; i++)
        {
            if (i != characterTile.Row)
            {
                targetList.Add(ActionUtils.InstantiateActionPosition(BoardNew.GetTileByCoordinates(i, characterTile.Column).gameObject.transform.position, attackCirclePrefab));
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
