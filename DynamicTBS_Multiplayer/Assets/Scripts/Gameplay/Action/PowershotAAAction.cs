using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject powershotUpPrefab;
    [SerializeField] private GameObject powershotDownPrefab;
    [SerializeField] private GameObject powershotLeftPrefab;
    [SerializeField] private GameObject powershotRightPrefab;

    private List<GameObject> powershotTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return powershotTargets; } }

    private Character characterInAction = null;

    public void CreateActionDestinations(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        powershotTargets = new List<GameObject>() {
            ActionUtils.InstantiateActionPosition(Board.FindPosition(characterTile.GetRow(), -1), powershotLeftPrefab),
            ActionUtils.InstantiateActionPosition(Board.FindPosition(characterTile.GetRow(), Board.boardSize), powershotRightPrefab),
            ActionUtils.InstantiateActionPosition(Board.FindPosition(-1, characterTile.GetColumn()), powershotDownPrefab),
            ActionUtils.InstantiateActionPosition(Board.FindPosition(Board.boardSize, characterTile.GetColumn()), powershotUpPrefab)
        };

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
            if (tile.GetCurrentInhabitant().isAttackableBy(characterInAction))
                tile.GetCurrentInhabitant().TakeDamage(PowershotAA.powershotDamage);
        }
        characterInAction.TakeDamage(PowershotAA.selfDamage);

        characterInAction.SetActiveAbilityOnCooldown();
        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(powershotTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }
}
