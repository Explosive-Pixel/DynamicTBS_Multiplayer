using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraftManager : MonoBehaviour
{
    private const int MaxDraftCount = 14;
    private static List<int> draftOrder = new List<int>() {3, 6, 7, 9, 11, 13};
    
    private Vector3 firstPosition = new Vector3(-6.5f, -3.5f, 0.998f);
    private float offset = 1f;
    private int draftCounter;

    private void Awake()
    {
        draftCounter = 0;
    }

    public void CreateCharacter()
    {
        if (draftCounter >= MaxDraftCount) return;
        
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        if (!PlayerManager.IsCurrentPlayer(buttonName)) return;
        
        Character character = CharacterFactory.CreateCharacter(buttonName, PlayerManager.GetCurrentPlayer());
        GameObject characterGameObject = character.GetCharacterGameObject();

        characterGameObject.transform.position = firstPosition;
        firstPosition.x += offset;

        DraftEvents.CharacterCreated(character);

        AdvanceDraftOrder();
    }

    private void AdvanceDraftOrder()
    {
        draftCounter += 1;

        if (draftOrder.Contains(draftCounter))
        {
            PlayerManager.NextPlayer();
            DraftEvents.ChangeDraftMessageText();
        }
            
        
        if (draftCounter >= MaxDraftCount)
        {
            DraftCompleted();
        }
    }

    private void DraftCompleted()
    {
        DraftEvents.EndDraft();
    }
}