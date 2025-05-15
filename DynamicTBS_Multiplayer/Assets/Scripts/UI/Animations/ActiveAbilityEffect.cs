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
            AnimateIndicator(actionMetadata.CharacterInitialPosition);
            AnimateIndicator(actionMetadata.ActionDestinationPosition);
            AnimateIndicator(actionMetadata.SecondActionDestinationPosition);
        }
    }

    private void AnimateIndicator(Vector3? position)
    {
        if (position.HasValue)
        {
            Tile tile = Board.GetTileByPosition(position.Value);
            AnimateIndicator(tile.IsOccupied() ? abilityCharacterPrefab : abilityTilePrefab, tile.transform.position, indicatorTime);
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= OnExecuteActiveAbilty;
    }
}
