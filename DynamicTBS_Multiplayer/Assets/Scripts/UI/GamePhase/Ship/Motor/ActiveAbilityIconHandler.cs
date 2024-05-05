using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityIconHandler : MonoBehaviour
{
    public static void ExecuteActiveAbility()
    {
        if (CharacterManager.SelectedCharacter == null || GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        ActionUtils.ResetActionDestinations();

        if (GameManager.IsSpectator() || CharacterManager.SelectedCharacter.Side == PlayerManager.ExecutingPlayer)
            CharacterManager.SelectedCharacter.ExecuteActiveAbility();
    }

    private void OnMouseDown()
    {
        AudioEvents.PressingButton();
        ExecuteActiveAbility();
    }
}
