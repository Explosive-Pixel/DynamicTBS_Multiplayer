using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraftManager : MonoBehaviour
{
    #region Draft Config

    public const int MaxDraftCount = 14;
    public static readonly List<int> draftOrder = new List<int>() {3, 6, 7, 9, 11, 13};
    private static List<Vector3> instantiationPositions = new List<Vector3>() {
        new Vector3(1f, 3.25f, 0.998f), 
        new Vector3(2f, 3.25f, 0.998f), 
        new Vector3(3f, 3.25f, 0.998f), 
        new Vector3(-1f, 2.3f, 0.998f), 
        new Vector3(-2f, 2.3f, 0.998f), 
        new Vector3(-3f, 2.3f, 0.998f), 
        new Vector3(1f, 1.4f, 0.998f), 
        new Vector3(-1f, 0.45f, 0.998f), 
        new Vector3(-2f, 0.45f, 0.998f), 
        new Vector3(1f, -0.45f, 0.998f), 
        new Vector3(2f, -0.45f, 0.998f), 
        new Vector3(-1f, -1.4f, 0.998f), 
        new Vector3(-2f, -1.4f, 0.998f), 
        new Vector3(1f, -2.35f, 0.998f) };

    #endregion

    private static int draftCounter;
    private static int draftOrderIndex;

    private void Awake()
    {
        draftCounter = 0;
        draftOrderIndex = 0;
    }

    public void CreateCharacter()
    {
        if (draftCounter >= MaxDraftCount) return;
        
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        if (!PlayerManager.IsCurrentPlayer(buttonName)) return;

        if (GameManager.gameType == GameType.multiplayer && Client.Instance.side != PlayerManager.GetCurrentPlayer().GetPlayerType())
            return;

        Enum.TryParse(buttonName.Split("_")[0], out CharacterType characterType);

        DraftCharacter(characterType, PlayerManager.GetCurrentPlayer());
    }

    public static void DraftCharacter(CharacterType type, Player side)
    {
        if (draftCounter >= MaxDraftCount) return;

        Character character = CharacterFactory.CreateCharacter(type, side);
        GameObject characterGameObject = character.GetCharacterGameObject();

        characterGameObject.transform.position = instantiationPositions[draftCounter];

        DraftEvents.CharacterCreated(character);

        AdvanceDraftOrder();
    }

    public static int GetCurrentDraftCount()
    {
        if(draftOrderIndex == 0)
        {
            return draftOrder[draftOrderIndex];
        } else if(draftOrderIndex == draftOrder.Count)
        {
            return MaxDraftCount - draftOrder[draftOrderIndex - 1];
        }
        return draftOrder[draftOrderIndex] - draftOrder[draftOrderIndex - 1];
    }

    private static void AdvanceDraftOrder()
    {
        draftCounter += 1;

        if (draftOrder.Contains(draftCounter))
        {
            draftOrderIndex++;
            PlayerManager.NextPlayer();
            DraftEvents.ChangeDraftMessageText();
        }
            
        
        if (draftCounter >= MaxDraftCount)
        {
            DraftCompleted();
        }
    }

    private static void DraftCompleted()
    {
        DraftEvents.EndDraft();
    }
}