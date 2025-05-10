using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityEffect : IndicatorEffect
{
    [SerializeField] private GameObject abilityCharacterPrefab;
    [SerializeField] private GameObject abilityTilePrefab;

    [SerializeField] private float indicatorTime;

    private void Awake()
    {
        GameplayEvents.OnFinishAction += OnExecuteActiveAbilty;
    }

    private void OnExecuteActiveAbilty(ActionMetadata actionMetadata)
    {
        if (actionMetadata.ExecutedActionType == ActionType.ActiveAbility)
        {
            Tile initialTile = Board.GetTileByPosition(actionMetadata.CharacterInitialPosition.Value);
            Tile destinationTile = Board.GetTileByPosition(actionMetadata.ActionDestinationPosition.Value);

            AnimateIndicator(initialTile.IsOccupied() ? abilityCharacterPrefab : abilityTilePrefab, initialTile.transform.position, indicatorTime);
            AnimateIndicator(destinationTile.IsOccupied() ? abilityCharacterPrefab : abilityTilePrefab, destinationTile.transform.position, indicatorTime);
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= OnExecuteActiveAbilty;
    }
}
