using UnityEngine;

public class MoveEffect : IndicatorEffect
{
    [SerializeField] private GameObject movePrefab_blue;
    [SerializeField] private GameObject movePrefab_pink;

    [SerializeField] private float indicatorTime;

    private void Awake()
    {
        GameplayEvents.OnFinishAction += OnMoveCharacter;
    }

    private void OnMoveCharacter(ActionMetadata actionMetadata)
    {
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        if (actionMetadata.ExecutedActionType == ActionType.Move)
        {
            GameObject prefab = actionMetadata.CharacterInAction.Side == PlayerType.blue ? movePrefab_blue : movePrefab_pink;
            AnimateIndicator(prefab, actionMetadata.CharacterInitialPosition.Value, indicatorTime);
            AnimateIndicator(prefab, actionMetadata.ActionDestinationPosition.Value, indicatorTime);
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= OnMoveCharacter;
    }
}
