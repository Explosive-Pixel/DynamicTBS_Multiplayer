using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    private void SendPerformActionMessage(ActionMetadata actionMetadata)
    {
        if (Client.ShouldSendMessage(actionMetadata.ExecutingPlayer))
        {
            Tile characterInitialTile = actionMetadata.CharacterInitialPosition != null ? Board.GetTileByPosition(actionMetadata.CharacterInitialPosition.Value) : null;
            List<Tile> actionsDestinationTiles = new();
            if (actionMetadata.ActionDestinationPosition != null)
            {
                actionsDestinationTiles.Add(Board.GetTileByPosition(actionMetadata.ActionDestinationPosition.Value));
            }
            if (actionMetadata.SecondActionDestinationPosition != null)
            {
                actionsDestinationTiles.Add(Board.GetTileByPosition(actionMetadata.SecondActionDestinationPosition.Value));
            }

            ActionDestination[] actionDestinations = actionsDestinationTiles.ConvertAll(tile => new ActionDestination()
            {
                tileName = tile.Name,
                x = tile.transform.position.x,
                y = tile.transform.position.y
            }).ToArray();

            Client.SendToServer(new WSMsgPerformAction
            {
                playerId = actionMetadata.ExecutingPlayer,
                characterType = actionMetadata.CharacterInAction != null ? (int)actionMetadata.CharacterInAction.CharacterType : -1,
                characterInitialTileName = characterInitialTile != null ? characterInitialTile.Name : null,
                characterX = actionMetadata.CharacterInitialPosition != null ? actionMetadata.CharacterInitialPosition.Value.x : 0f,
                characterY = actionMetadata.CharacterInitialPosition != null ? actionMetadata.CharacterInitialPosition.Value.y : 0f,
                actionType = actionMetadata.ExecutedActionType,
                playerActionType = actionMetadata.PlayerActionType != null ? actionMetadata.PlayerActionType.Value : 0,
                actionDestinations = actionDestinations
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
        if (Client.IsAdmin)
        {
            Client.SendToServer(new WSMsgUpdateServer
            {
                currentPlayer = currentPlayer,
                gamePhase = GameManager.CurrentGamePhase
            });
        }
    }

    private void SendGameOverMessage(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (Client.IsAdmin)
        {
            Client.SendToServer(new WSMsgGameOver { winner = winner ?? PlayerType.none, gameOverCondition = endGameCondition });
        }
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
