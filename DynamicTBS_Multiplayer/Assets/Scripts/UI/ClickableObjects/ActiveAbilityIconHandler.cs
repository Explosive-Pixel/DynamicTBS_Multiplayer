using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityIconHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    public void OnClick()
    {
        if (GameManager.gamePhase == GamePhase.GAMEPLAY)
            UIClickHandler.CurrentCharacter.ExecuteActiveAbility();
    }
}
