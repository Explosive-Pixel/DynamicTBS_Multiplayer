using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DraftCharacterButtonHandler : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;
    [SerializeField] private GameObject pinkHoverObject;
    [SerializeField] private GameObject blueHoverObject;

    private void OnMouseDown()
    {
        if (!PlayerManager.ClientIsCurrentPlayer() || GameplayManager.gameIsPaused)
            return;

        AudioEvents.PressingButton();

        DraftManager.DraftCharacter(characterType, PlayerManager.CurrentPlayer);
    }

    private void OnMouseEnter()
    {
        pinkHoverObject.SetActive(PlayerManager.CurrentPlayer == PlayerType.pink);
        blueHoverObject.SetActive(PlayerManager.CurrentPlayer == PlayerType.blue);
    }

    private void OnMouseExit()
    {
        pinkHoverObject.SetActive(false);
        blueHoverObject.SetActive(false);
    }
}
