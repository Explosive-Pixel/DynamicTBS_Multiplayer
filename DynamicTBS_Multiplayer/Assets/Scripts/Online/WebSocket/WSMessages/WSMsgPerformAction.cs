using System;
using System.Linq;
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
        Action action = ToAction();

        if (actionSteps == null || actionSteps.Length == 0)
        {
            PlayerActionUtils.ExecuteAction(action);
        }
        else
        {
            Character currentlySelectedCharacter = CharacterManager.SelectedCharacter;

            ActionHandler.Instance.ExecuteAction(action);

            if (currentlySelectedCharacter != null && !currentlySelectedCharacter.IsDead())
                currentlySelectedCharacter.Select();
            else
                GameplayEvents.ChangeCharacterSelection(null);

        }
    }

    public static void SendPerformActionMessage(Action action)
    {
        if (!Client.IsLoadingGame)
        {
            Client.SendToServer(FromAction(action));
        }
    }

    private static WSMsgPerformAction FromAction(Action action)
    {
        return new WSMsgPerformAction
        {
            playerId = action.ExecutingPlayer,
            playerActionType = action.PlayerActionType != null ? action.PlayerActionType.Value : 0,
            actionSteps = (action.ActionSteps != null ? action.ActionSteps.ConvertAll(actionStep =>
            {
                Tile characterInitialTile = actionStep.CharacterInitialPosition != null ? Board.GetTileByPosition(actionStep.CharacterInitialPosition.Value) : null;
                Tile actionDestinationTile = actionStep.ActionDestinationPosition != null ? Board.GetTileByPosition(actionStep.ActionDestinationPosition.Value) : null;
                return new ActionStepDto()
                {
                    characterType = (int)actionStep.CharacterInAction.CharacterType,
                    characterInitialTileName = characterInitialTile != null ? characterInitialTile.Name : null,
                    characterX = actionStep.CharacterInitialPosition != null ? actionStep.CharacterInitialPosition.Value.x : 0f,
                    characterY = actionStep.CharacterInitialPosition != null ? actionStep.CharacterInitialPosition.Value.y : 0f,
                    actionType = actionStep.ActionType,
                    actionDestinationTileName = actionDestinationTile != null ? actionDestinationTile.Name : null,
                    actionDestinationX = actionStep.ActionDestinationPosition != null ? actionStep.ActionDestinationPosition.Value.x : 0f,
                    actionDestinationY = actionStep.ActionDestinationPosition != null ? actionStep.ActionDestinationPosition.Value.y : 0f
                };
            }) : new()).ToArray()
        };
    }

    private Action ToAction()
    {
        int index = 0;
        return new Action()
        {
            ExecutingPlayer = playerId,
            PlayerActionType = playerActionType,
            ActionSteps = actionSteps != null ? actionSteps.ToList().ConvertAll(actionStep =>
            {
                index++;
                Debug.Log("Character Position: " + actionStep.characterX + ", " + actionStep.characterY);
                Character characterInAction = CharacterManager.GetCharacterByPosition(new Vector3(actionStep.characterX, actionStep.characterY, 0));
                Debug.Log("CharacterInAction: " + characterInAction);
                return new ActionStep()
                {
                    ActionType = actionStep.actionType,
                    CharacterInAction = characterInAction,
                    CharacterInitialPosition = new Vector3(actionStep.characterX, actionStep.characterY, 0),
                    ActionDestinationPosition = new Vector3(actionStep.actionDestinationX, actionStep.actionDestinationY, 0),
                    ActionFinished = index == actionSteps.Length
                };
            }) : new()
        };
    }
}
