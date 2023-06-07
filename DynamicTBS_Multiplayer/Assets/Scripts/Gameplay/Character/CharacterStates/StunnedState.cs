using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : State
{
    public const int StunDuration = 1;

    protected override int Duration { get { return StunDuration; } }
    private static readonly string blueStunPrefabPath = "EffectPrefabs/StunMarkerBluePrefab";
    private static readonly string pinkStunPrefabPath = "EffectPrefabs/StunMarkerPinkPrefab";

    public StunnedState(GameObject parent) : base(parent)
    {
    }

    protected override GameObject LoadPrefab(GameObject parent)
    {
        Character character = CharacterHandler.GetCharacterByGameObject(parent);

        if (character != null)
        {
            string prefabPath = character.GetSide().GetPlayerType() == PlayerType.blue ? blueStunPrefabPath : pinkStunPrefabPath;
            return Resources.Load<GameObject>(prefabPath);
        }

        return null;
    }
}
