using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

[CreateAssetMenu(menuName = "AI/Difficulty/Chaos")]
public class DifficultyChaos : AIDifficultySO
{
    public override AIAction CalculateBestMove()
    {
        SetParams();
        if (AvailableCharacters.Count > 0)
        {
            GetCharacterWithMoves();
        }
        
        ActionToTake.Type =
                (ActionType)RandomNumberGenerator.GetInt32((int)ActionType.Move, (int)ActionType.ActiveAbility + 1);
            if (ActionToTake.Type == ActionType.ActiveAbility && ActionToTake.Character.CanPerformActiveAbility())
            {
                ActionToTake.Character.ExecuteActiveAbility();
            }
            else
            {
                ActionUtils.InstantiateAllActionPositions(ActionToTake.Character);
            }
            
            ActionDestinations = ActionRegistry.GetActions().ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
            
            if (ActionDestinations.Count == 0)
            {
                ActionToTake.Character.ExecuteActiveAbility();
                ActionDestinations = ActionRegistry.GetActions().ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
            }
            ActionToTake.Target = ActionDestinations[RandomNumberGenerator.GetInt32(0, ActionDestinations.Count)];
            foreach (IAction action in ActionRegistry.GetActions())
            {
                if(action.ActionType == ActionToTake.Type)
                    ActionDestinations.AddRange(action.ActionDestinations);
            }
            
            return ActionToTake;
    }

    private void GetCharacterWithMoves()
    {
        do
        {
            ActionToTake.Character =
                AvailableCharacters[RandomNumberGenerator.GetInt32(0, AvailableCharacters.Count)];
        } while (!ActionToTake.Character.CanPerformAction());
    }
}