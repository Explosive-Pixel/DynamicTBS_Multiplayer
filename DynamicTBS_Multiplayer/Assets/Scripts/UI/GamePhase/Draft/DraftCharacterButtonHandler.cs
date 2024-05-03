using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftCharacterButtonHandler : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;

    private void OnMouseDown()
    {
        if (PlayerManager.ClientIsCurrentPlayer())
        {
            AudioEvents.PressingButton();

            DraftManager.DraftCharacter(characterType, PlayerManager.CurrentPlayer);
        }
    }
}
