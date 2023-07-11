using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftButtonHandler : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;
    [SerializeField] private PlayerType side;

    public void DraftCharacter()
    {
        AudioEvents.PressingButton();

        if (!PlayerManager.IsCurrentPlayer(side)) return;

        if (!PlayerManager.ClientIsCurrentPlayer())
            return;

        DraftManager.DraftCharacter(characterType, side);
    }
}
