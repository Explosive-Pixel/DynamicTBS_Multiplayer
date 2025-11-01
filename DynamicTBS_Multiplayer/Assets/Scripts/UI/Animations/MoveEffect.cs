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

    private void OnMoveCharacter(Action action)
    {
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        action.ActionSteps?.ForEach(actionStep =>
        {
            if (actionStep.ActionType == ActionType.Move)
            {
                GameObject prefab = actionStep.CharacterInAction.Side == PlayerType.blue ? movePrefab_blue : movePrefab_pink;
                AnimateIndicator(prefab, actionStep.CharacterInitialPosition.Value, indicatorTime);
                AnimateIndicator(prefab, actionStep.ActionDestinationPosition.Value, indicatorTime);
            }
        });
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= OnMoveCharacter;
    }
}
