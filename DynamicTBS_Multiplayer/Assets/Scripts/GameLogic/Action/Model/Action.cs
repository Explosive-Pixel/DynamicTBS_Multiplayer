using System.Collections.Generic;

public class Action
{
    public PlayerType ExecutingPlayer { get; set; }
    public PlayerActionType? PlayerActionType { get; set; }
    public List<ActionStep> ActionSteps { get; set; }

    public ActionStep FindActionStep(ActionType actionType, Character character)
    {
        if (ActionSteps != null)
        {
            return ActionSteps.Find(step => step.ActionType == actionType && step.CharacterInAction == character);
        }

        return null;
    }
}
