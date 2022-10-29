using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();
    public static event GameplayPhase OnGameplayPhaseStart;

    public delegate void FinishAction(UIActionType type);
    public static event FinishAction OnFinishAction;

    public delegate void ExecuteActiveAbility();
    public static event ExecuteActiveAbility OnExecuteActiveAbility;

    public delegate void GameOver(PlayerType winner);
    public static event GameOver OnGameOver;

    public delegate void CharacterSelection(Character character);
    public static event CharacterSelection OnCharacterSelectionChange;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart != null)
            OnGameplayPhaseStart();
    }

    public static void ActionFinished(UIActionType type)
    {
        if (OnFinishAction != null)
            OnFinishAction(type);
    }

    public static void ExecuteActiveAbilityStarted()
    {
        if (OnExecuteActiveAbility != null)
            OnExecuteActiveAbility();
    }

    public static void GameIsOver(PlayerType winner)
    {
        if (OnGameOver != null) 
        {
            OnGameOver(winner);
        }
    }

    public static void ChangeCharacterSelection(Character character)
    {
        if (OnCharacterSelectionChange != null)
            OnCharacterSelectionChange(character);
    }
}