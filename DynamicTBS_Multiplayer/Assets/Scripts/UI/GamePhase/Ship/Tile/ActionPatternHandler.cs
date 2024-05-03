using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPatternHandler : MonoBehaviour
{
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
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY || CharacterManager.SelectedCharacter == null)
            return;

        ActionUtils.ResetActionDestinations();

        if (actionType == ActionType.ActiveAbility)
            CharacterManager.SelectedCharacter.ActiveAbility.ShowActionPattern();
        else
        {
            IAction action = ActionRegistry.GetActions().Find(action => action.ActionType == actionType);
            if (action != null)
            {
                action.ShowActionPattern(CharacterManager.SelectedCharacter);
            }
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.HideAllActionPatterns();

        if (CharacterManager.SelectedCharacter != null)
            CharacterManager.SelectedCharacter.Select();
    }
}
