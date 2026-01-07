using System;

[Serializable]
public class WSMsgDraftCharacter : WSMessage
{
    public PlayerType playerId;
    public CharacterType characterType;

    public WSMsgDraftCharacter()
    {
        code = WSMessageCode.WSMsgDraftCharacterCode;
    }

    public override void HandleMessage()
    {
        DraftManager.ExecuteDraftCharacter(characterType, playerId);
    }

    public static void SendDraftCharacterMessage(CharacterType type, PlayerType side)
    {
        if (!Client.IsLoadingGame)
        {
            Client.SendToServer(new WSMsgDraftCharacter
            {
                playerId = side,
                characterType = type
            });
        }
    }
}