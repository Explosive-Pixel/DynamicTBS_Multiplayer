using UnityEngine;

public class MessageSender : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void SendDraftCharacterMessage(Character character)
    {
        if (Client.ShouldSendMessage(character.Side) && character.CharacterType != CharacterType.CaptainChar)
        {
            Client.SendToServer(new WSMsgDraftCharacter
            {
                playerId = character.Side,
                characterType = character.CharacterType
            });
        }
    }

    private void SendPerformActionMessage(Action action)
    {
        if (Client.ShouldSendMessage(action.ExecutingPlayer))
        {
            Client.SendToServer(new WSMsgPerformAction
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
            });
        }
    }

    private void SendUIActionMessage(PlayerType player, UIAction uiAction)
    {
        if (Client.ShouldSendMessage(player))
        {
            if (uiAction == UIAction.PAUSE_GAME || uiAction == UIAction.UNPAUSE_GAME)
            {
                SendPauseGameMessage(uiAction == UIAction.PAUSE_GAME);
                return;
            }

            Client.SendToServer(new WSMsgUIAction
            {
                playerId = player,
                uiAction = uiAction
            });
        }
    }

    private void SendPauseGameMessage(bool paused)
    {
        Client.SendToServer(new WSMsgPauseGame
        {
            pause = paused
        });
    }

    private void SendUpdateServerMessage(PlayerType currentPlayer)
    {
        if (Client.Role != ClientType.PLAYER || Client.IsLoadingGame)
            return;

        Client.SendToServer(new WSMsgUpdateServer
        {
            currentPlayer = currentPlayer,
            gamePhase = GameManager.CurrentGamePhase
        });
    }

    private void SendGameOverMessage(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (Client.Role != ClientType.PLAYER || Client.IsLoadingGame)
            return;

        Client.SendToServer(new WSMsgGameOver { winner = winner ?? PlayerType.none, gameOverCondition = endGameCondition });
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += SendDraftCharacterMessage;
        GameplayEvents.OnFinishAction += SendPerformActionMessage;
        GameplayEvents.OnExecuteUIAction += SendUIActionMessage;
        GameplayEvents.OnCurrentPlayerChanged += SendUpdateServerMessage;
        GameplayEvents.OnGameOver += SendGameOverMessage;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= SendDraftCharacterMessage;
        GameplayEvents.OnFinishAction -= SendPerformActionMessage;
        GameplayEvents.OnExecuteUIAction -= SendUIActionMessage;
        GameplayEvents.OnCurrentPlayerChanged -= SendUpdateServerMessage;
        GameplayEvents.OnGameOver -= SendGameOverMessage;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
