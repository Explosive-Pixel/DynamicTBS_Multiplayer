using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DraftEvents
{
    public delegate void CharacterCreation(Character character);
    public static event CharacterCreation OnCharacterCreated;

    public delegate void DraftMessageText();
    public static event DraftMessageText OnDraftMessageTextChange;

    public static void CharacterCreated(Character character)
    {
        if (OnCharacterCreated != null)
            OnCharacterCreated(character);
    }

    public static void ChangeDraftMessageText()
    {
        if (OnDraftMessageTextChange != null)
            OnDraftMessageTextChange();
    }
}