using System;
using UnityEngine;

[Serializable]
public class WSMsgPerformAction : WSMessage
{
    public PlayerType playerId;
    public PlayerActionType playerActionType;
    public ActionStepDto[] actionSteps;

    public WSMsgPerformAction()
    {
        code = WSMessageCode.WSMsgPerformActionCode;
    }

    public override void HandleMessage()
    {
        if (Client.ShouldReadMessage(playerId))
        {
            Character currentlySelectedCharacter = CharacterManager.SelectedCharacter;
            GameplayEvents.ChangeCharacterSelection(null);

            if (actionSteps == null || actionSteps.Length == 0)
            {
                PlayerActionUtils.ExecuteAction(PlayerActionRegistry.GetAction(playerActionType), playerId);
            }
            else
            {
                {
                    int i = 0;
                    while (i < actionSteps.Length)
                    {
                        ActionStepDto actionStep = actionSteps[i];
                        Character character = CharacterManager.GetCharacterByPosition(new Vector3(actionStep.characterX, actionStep.characterY, 0));
                        if (actionStep.actionType == ActionType.ActiveAbility)
                        {
                            character.ActiveAbility.Execute();
                        }
                        else
                        {
                            ActionHandler.Instance.InstantiateAllActionPositions(character);
                        }

                        ActionHandler.Instance.ExecuteAction(new Vector3(actionStep.actionDestinationX, actionStep.actionDestinationY, 0));

                        while ((i + 1) < actionSteps.Length && actionSteps[i + 1].actionType == actionStep.actionType && actionSteps[i + 1].characterInitialTileName == actionStep.characterInitialTileName)
                        {
                            ActionHandler.Instance.ExecuteAction(new Vector3(actionSteps[i + 1].actionDestinationX, actionSteps[i + 1].actionDestinationY, 0));
                            i++;
                        }

                        i++;
                    }
                }
            }

            if (currentlySelectedCharacter != null && !currentlySelectedCharacter.IsDead())
                currentlySelectedCharacter.Select();
            else
                GameplayEvents.ChangeCharacterSelection(null);
        }
    }
}
