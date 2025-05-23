using System;
using UnityEngine;

[Serializable]
public class WSMsgPerformAction : WSMessage
{
    public PlayerType playerId;
    public int characterType;
    public string characterInitialTileName;
    public float characterX;
    public float characterY;
    public ActionType actionType;
    public PlayerActionType playerActionType;
    public bool hasDestination;
    public string actionDestinationTileName;
    public float destinationX;
    public float destinationY;

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

            if (actionType == ActionType.PlayerAction)
            {
                PlayerActionUtils.ExecuteAction(PlayerActionRegistry.GetAction(playerActionType), playerId);
            }
            else
            {
                Character character = CharacterManager.GetCharacterByPosition(new Vector3(characterX, characterY, 0));
                if (actionType == ActionType.ActiveAbility)
                {
                    character.ActiveAbility.Execute();
                }
                else
                {
                    ActionUtils.InstantiateAllActionPositions(character);
                }

                if (hasDestination)
                {
                    ActionUtils.ExecuteAction(new Vector3(destinationX, destinationY, 0));
                }
            }

            if (currentlySelectedCharacter != null && !currentlySelectedCharacter.IsDead())
                currentlySelectedCharacter.Select();
            else
                GameplayEvents.ChangeCharacterSelection(null);
        }
    }
}
