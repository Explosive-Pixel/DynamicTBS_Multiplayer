using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : State
{
    public const int StunDuration = 1;

    protected override int Duration { get { return StunDuration; } }
    private static readonly string blueStunSpritePath = "EffectSprites/StunMarker_Blue";
    private static readonly string pinkStunSpritePath = "EffectSprites/StunMarker_Pink";

    public StunnedState(GameObject parent) : base(parent)
    {
    }

    protected override Sprite LoadSprite(GameObject parent)
    {
        Character character = CharacterHandler.GetCharacterByGameObject(parent);

        if (character != null)
        {
            string spritePath = character.GetSide().GetPlayerType() == PlayerType.blue ? blueStunSpritePath : pinkStunSpritePath;
            return Resources.Load<Sprite>(spritePath);
        }

        return null;
    }
}
