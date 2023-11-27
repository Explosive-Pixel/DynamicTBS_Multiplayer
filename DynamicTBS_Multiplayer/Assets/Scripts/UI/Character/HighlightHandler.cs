using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightHandler : MonoBehaviour
{
    private Character character;

    private void Awake()
    {
        character = gameObject.GetComponentInParent<Character>();
        Highlight(false);

        GameplayEvents.OnCharacterSelectionChange += HighlightCharacter;
    }

    private void HighlightCharacter(Character character)
    {
        Highlight(character == this.character);
    }

    private void Highlight(bool highlight)
    {
        gameObject.SetActive(highlight);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= HighlightCharacter;
    }
}
