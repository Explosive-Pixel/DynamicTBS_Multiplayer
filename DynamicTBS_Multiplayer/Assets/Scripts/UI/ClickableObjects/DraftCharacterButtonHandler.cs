using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftCharacterButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private CharacterType characterType;

    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    public void OnClick()
    {
        if (Input.GetKey(KeyCode.Space))
            return;

        AudioEvents.PressingButton();

        DraftManager.DraftCharacter(characterType, PlayerManager.CurrentPlayer);
    }
}
