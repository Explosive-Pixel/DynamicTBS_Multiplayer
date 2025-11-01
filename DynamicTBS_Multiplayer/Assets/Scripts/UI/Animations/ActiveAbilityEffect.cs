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

    private void OnExecuteActiveAbilty(Action action)
    {
        action.ActionSteps?.ForEach(actionStep =>
            {
                if (actionStep.ActionType == ActionType.ActiveAbility)
                {
                    AnimateIndicator(actionStep.CharacterInitialPosition);
                    AnimateIndicator(actionStep.ActionDestinationPosition);
                }
            });
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
