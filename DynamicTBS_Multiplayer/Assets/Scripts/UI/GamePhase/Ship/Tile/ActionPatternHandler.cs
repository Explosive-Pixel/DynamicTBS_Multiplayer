using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionPatternHandler : MonoBehaviour
{
    [SerializeField] private GameObject actionRegistry;

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += RegisterActionPatterns;
    }

    public void ShowMovePattern()
    {
        ShowActionPattern(ActionType.Move);
    }

    public void ShowAttackPattern()
    {
        ShowActionPattern(ActionType.Attack);
    }

    public void ShowActiveAbilityPattern()
    {
        ShowActionPattern(ActionType.ActiveAbility);
    }

    private void ShowActionPattern(ActionType actionType)
    {
        if (CharacterManager.SelectedCharacter == null || CharacterManager.SelectedCharacter.CurrentTile == null)
            return;

        ActionHandler.Instance.ResetActionDestinations();

        if (actionType == ActionType.ActiveAbility)
            CharacterManager.SelectedCharacter.ActiveAbility.ShowActionPattern();
        else
        {
            ActionRegistry.ShowActionPattern(actionType, CharacterManager.SelectedCharacter);
        }
    }

    public void HideActionPattern()
    {
        ActionHandler.Instance.HideAllActionPatterns();

        if (CharacterManager.SelectedCharacter != null)
            CharacterManager.SelectedCharacter.Select();
    }

    private void RegisterActionPatterns(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.PLACEMENT)
            return;

        List<IAction> actions = actionRegistry.GetComponentsInChildren<IAction>().ToList();
        actions.ForEach(action => ActionRegistry.RegisterPatternAction(action));
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= RegisterActionPatterns;
    }
}
