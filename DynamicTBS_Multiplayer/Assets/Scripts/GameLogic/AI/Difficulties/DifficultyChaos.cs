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
        else
        {
                ActionToTake.Type = ActionType.Skip;
                return ActionToTake;
        }

        ActionToTake.Type =
                (ActionType)RandomNumberGenerator.GetInt32((int)ActionType.Move, (int)ActionType.ActiveAbility + 1);
            if (ActionToTake.Type != ActionType.ActiveAbility)
            {
                ActionUtils.InstantiateAllActionPositions(ActionToTake.Character);
            }
            ActionToTake.Target = ActionDestinations[RandomNumberGenerator.GetInt32(0, ActionDestinations.Count)];
        

        return ActionToTake;
    }

    private void GetCharacterWithMoves()
    {
        do
        {
            ActionToTake.Character =
                AvailableCharacters[RandomNumberGenerator.GetInt32(0, AvailableCharacters.Count)];
            ActionUtils.InstantiateAllActionPositions(ActionToTake.Character);
            ActionDestinations = ActionRegistry.GetActions()
                .ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
        } while (ActionDestinations.Count == 0);
    }
}