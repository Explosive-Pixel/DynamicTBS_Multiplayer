using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityIconHandler : MonoBehaviour, IClickableObject
{
    public void OnClick()
    {
        UIClickHandler.CurrentCharacter.ExecuteActiveAbility();
    }
}
