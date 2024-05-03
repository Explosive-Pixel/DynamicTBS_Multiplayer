using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftCharacterButtonHandler : MonoBehaviour
{
    [SerializeField] private CharacterType characterType;
    [SerializeField] private GameObject pinkHoverObject;
    [SerializeField] private GameObject blueHoverObject;

    private void Start()
    {
        OnMouseExit();
    }

    private void OnMouseDown()
    {
        if (PlayerManager.ClientIsCurrentPlayer())
        {
            AudioEvents.PressingButton();

            DraftManager.DraftCharacter(characterType, PlayerManager.CurrentPlayer, GetHoverObject(PlayerManager.CurrentPlayer));
        }
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

    private GameObject GetHoverObject(PlayerType side)
    {
        if (side == PlayerType.pink)
            return pinkHoverObject;
        if (side == PlayerType.blue)
            return blueHoverObject;

        return null;
    }
}
