using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DraftEvents
{
    public delegate void DraftPhase();
    public static event DraftPhase OnStartDraft;
    public static event DraftPhase OnEndDraft;

    public delegate void CharacterCreation(Character character);
    public static event CharacterCreation OnCharacterCreated;

    public delegate void CharacterList(List<Character> characters);
    public static event CharacterList OnDeliverCharacterList;

    public delegate void DraftMessageText();
    public static event DraftMessageText OnDraftMessageTextChange;

    public static void StartDraft()
    {
        if (OnStartDraft != null)
            OnStartDraft();
    }

    public static void EndDraft()
    {
        if (OnEndDraft != null)
            OnEndDraft();
    }

    public static void CharacterCreated(Character character)
    {
        if (OnCharacterCreated != null)
            OnCharacterCreated(character);
    }

    public static void DeliverCharacterList(List<Character> characters)
    {
        if (OnDeliverCharacterList != null)
            OnDeliverCharacterList(characters);
    }

    public static void ChangeDraftMessageText()
    {
        if (OnDraftMessageTextChange != null)
            OnDraftMessageTextChange();
    }
}