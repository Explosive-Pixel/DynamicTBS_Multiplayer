using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (Client.ShouldReadMessage(playerId))
        {
            DraftManager.DraftCharacter(characterType, playerId);
        }
    }
}