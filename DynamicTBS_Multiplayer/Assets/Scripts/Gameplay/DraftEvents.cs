using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DraftEvents
{
    public delegate void DraftEnd();
    public static event DraftEnd OnEndDraft;

    
    public delegate void CharacterCreation(Character character);
    public static event CharacterCreation OnCharacterCreated;

    public delegate void CharacterList(List<Character> characters);
    public static event CharacterList OnDeliverCharacterList;

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
}