using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "AI/Difficulty/Chaos")]
public class DifficultyChaos : AIDifficultySO
{
    public override AIAction CalculateBestMove()
    {
        SetParams();
        if (AvailableCharacters.Count > 0)
        {
            ActionToTake.Character =
                AvailableCharacters[RandomNumberGenerator.GetInt32(0, AvailableCharacters.Count)];
        }
        else
        {
            ActionToTake.Type = ActionType.Skip;
            return ActionToTake;
        }
        ActionToTake.Type  = (ActionType) RandomNumberGenerator.GetInt32((int) ActionType.Move, (int) ActionType.ActiveAbility);
        if (ActionToTake.Type != ActionType.ActiveAbility)
        {
            ActionUtils.InstantiateAllActionPositions(ActionToTake.Character);

        }
        List<GameObject> actionDestinations = ActionRegistry.GetActions().ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
        ActionToTake.Target = actionDestinations[RandomNumberGenerator.GetInt32(0, actionDestinations.Count)];
        return ActionToTake;
    }
}