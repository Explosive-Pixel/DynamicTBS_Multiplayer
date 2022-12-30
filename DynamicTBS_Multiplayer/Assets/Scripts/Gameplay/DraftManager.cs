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

    #endregion

    private static Vector3 firstPosition = new Vector3(-6.5f, -3.5f, 0.998f);
    private static float offset = 1f;
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

        characterGameObject.transform.position = firstPosition;
        firstPosition.x += offset;

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