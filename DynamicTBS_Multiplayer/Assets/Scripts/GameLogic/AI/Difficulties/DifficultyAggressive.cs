using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Difficulty/Aggressive")]
public class DifficultyAggressive : AIDifficultySO
{
    private bool canAttack = false;
    
    public override AIAction CalculateBestMove()
    {
        SetParams();
        if (AvailableCharacters.Count > 0)
        {
            if (!GetCharacterWithAttackMoves())
            {
                GetCharacterWithMoves();
                ActionToTake.Type =
                    (ActionType)RandomNumberGenerator.GetInt32((int)ActionType.Move, (int)ActionType.ActiveAbility + 1);
                if ((ActionToTake.Type == ActionType.ActiveAbility && ActionToTake.Character.CanPerformActiveAbility()) || ActionDestinations.Count == 0)
                {
                    ActionToTake.Character.ExecuteActiveAbility();
                    ActionDestinations = ActionRegistry.GetActions().ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
                }
            }
            else
            {
                ActionToTake.Type = ActionType.Attack;
            }
        }
        ActionToTake.Target = ActionDestinations[RandomNumberGenerator.GetInt32(0, ActionDestinations.Count)];
        Reset();
        return ActionToTake;
    }
    
    private bool GetCharacterWithAttackMoves()
    {
        List<Character> tempChars = new List<Character>(AvailableCharacters);
        do
        {
            ActionToTake.Character =
                tempChars[RandomNumberGenerator.GetInt32(0, tempChars.Count)];
            ActionUtils.InstantiateAllActionPositions(ActionToTake.Character);
            foreach (IAction action in ActionRegistry.GetActions())
            {
                if (action.ActionType == ActionType.Attack && action.ActionDestinations.Count > 0)
                {
                    ActionDestinations.AddRange(action.ActionDestinations);
                    canAttack = true;
                }
            }
            tempChars.Remove(ActionToTake.Character);
        } while (!(ActionToTake.Character.CanPerformAction() && canAttack) && tempChars.Count > 0);

        return canAttack;
    }
    
    private void GetCharacterWithMoves()
    {
        List<Character> tempChars = new List<Character>(AvailableCharacters);
        do
        {
            ActionToTake.Character =
                tempChars[RandomNumberGenerator.GetInt32(0, tempChars.Count)];
            ActionUtils.InstantiateAllActionPositions(ActionToTake.Character);
            tempChars.Remove(ActionToTake.Character);
        } while (!ActionToTake.Character.CanPerformAction());
        ActionDestinations = ActionRegistry.GetActions().ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
    }

    private void Reset()
    {
        canAttack = false;
        ActionDestinations.Clear();
    }
    
}
