using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAAHandler : MonoBehaviour
{
    private Board board;

    private bool waitForHealTarget;

    private void Awake()
    {
        SubscribeEvents();
        waitForHealTarget = false;
    }

    public void ExecuteHealAA(Character character) 
    {
        if (board == null)
            board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        Tile characterTile = board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> healTiles = board.GetTilesOfNearestCharactersOfSideWithinRadius(characterTile, character.GetSide().GetPlayerType(), HealAA.healingRange)
            .FindAll(tile => tile.GetCurrentInhabitant() != null && tile.GetCurrentInhabitant().CanBeHealed() && !tile.GetCurrentInhabitant().HasFullHP());

        List<Vector3> healPositions = healTiles.ConvertAll(tile => tile.GetPosition());

        // TODO: Filter characters with max health

        UIEvents.PassActionPositionsList(healPositions, UIActionType.ActiveAbility_Heal);
        waitForHealTarget = true;
    }

    private void PerformHeal(Vector3 position, UIActionType type)
    {
        if (waitForHealTarget && type == UIActionType.ActiveAbility_Heal)
        {
            Tile tile = board.GetTileByPosition(position);
            if (tile != null)
            {
                Character characterToHeal = tile.GetCurrentInhabitant();
                characterToHeal.Heal(HealAA.healingPoints);
            }
            waitForHealTarget = false;
            GameplayEvents.ActionFinished(UIActionType.ActiveAbility_Heal);
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        UIEvents.OnPassActionDestination += PerformHeal;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassActionDestination -= PerformHeal;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

}
