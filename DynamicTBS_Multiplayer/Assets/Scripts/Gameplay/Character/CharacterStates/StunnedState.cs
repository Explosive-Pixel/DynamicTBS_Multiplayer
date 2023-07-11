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
        CharacterMB character = parent.GetComponent<CharacterMB>();

        if (character != null)
        {
            string prefabPath = character.Side == PlayerType.blue ? blueStunPrefabPath : pinkStunPrefabPath;
            return Resources.Load<GameObject>(prefabPath);
        }

        return null;
    }
}
