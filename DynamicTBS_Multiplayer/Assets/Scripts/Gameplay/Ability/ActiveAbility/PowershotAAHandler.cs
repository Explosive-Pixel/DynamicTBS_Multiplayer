using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAAHandler : MonoBehaviour
{
    private Board board;

    private Character currentlySelectedCharacter = null;

    private void Awake()
    {
        SubscribeEvents();
    }

    public void ExecutePowershotAA(Character character)
    {
        if (board == null)
            board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        Tile characterTile = board.GetTileByCharacter(character);

        List<Vector3> shootDirectionPositions = new List<Vector3>() 
            { 
                Board.FindPosition(characterTile.GetRow(), -1), 
                Board.FindPosition(characterTile.GetRow(), Board.boardSize),
                Board.FindPosition(-1, characterTile.GetColumn()),
                Board.FindPosition(Board.boardSize, characterTile.GetColumn())
            };

        UIEvents.PassActionPositionsList(shootDirectionPositions, UIActionType.ActiveAbility_Powershot);
        currentlySelectedCharacter = character;
    }

    private void PerformPowershot(Vector3 position, UIActionType type)
    {
        if (currentlySelectedCharacter != null && type == UIActionType.ActiveAbility_Powershot)
        {
            Tile characterTile = board.GetTileByCharacter(currentlySelectedCharacter);
            Vector3 shooterPosition = characterTile.GetPosition();

            Vector3 shootDirection = Vector3.up;
            if (position.y == shooterPosition.y) 
            {
                if(position.x < shooterPosition.x)
                    shootDirection = Vector3.left;
                else
                    shootDirection = Vector3.right;
            }
            else
            {
                if(position.y < shooterPosition.y)
                {
                    shootDirection = Vector3.down;
                }
            }

            List<Tile> hitCharacterTiles = board.GetAllOccupiedTilesInOneDirection(characterTile, shootDirection);

            foreach(Tile tile in hitCharacterTiles)
            {
                tile.GetCurrentInhabitant().TakeDamage(PowershotAA.powershotDamage);
            }
            currentlySelectedCharacter.TakeDamage(PowershotAA.selfDamage);

            currentlySelectedCharacter = null;
            GameplayEvents.ActionFinished(UIActionType.ActiveAbility_Powershot);
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        UIEvents.OnPassActionDestination += PerformPowershot;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassActionDestination -= PerformPowershot;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
