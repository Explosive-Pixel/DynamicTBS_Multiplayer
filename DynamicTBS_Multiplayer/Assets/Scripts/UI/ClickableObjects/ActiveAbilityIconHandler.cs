using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityIconHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    public void OnClick()
    {
        if (UIClickHandler.CurrentCharacter == null || GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        ActionUtils.ResetActionDestinations();

        if (GameManager.IsSpectator() || UIClickHandler.CurrentCharacter.Side == PlayerManager.ExecutingPlayer)
            UIClickHandler.CurrentCharacter.ExecuteActiveAbility();
    }
}
