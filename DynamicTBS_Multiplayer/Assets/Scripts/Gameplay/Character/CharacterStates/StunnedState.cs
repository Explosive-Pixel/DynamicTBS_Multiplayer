using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class StunnedState : State
{
    public const int StunDuration = 1;

    protected override int Duration { get { return StunDuration; } }
    private static readonly string blueStunPrefabPath = "EffectPrefabs/StunMarkerBluePrefab";
    private static readonly string pinkStunPrefabPath = "EffectPrefabs/StunMarkerPinkPrefab";

    public StunnedState(GameObject parent) : base(parent)
    {
        Character character = parent.GetComponent<Character>();

        var defaultIsDisabled = character.isDisabled;
        character.isDisabled = () =>
        {
            if (character.IsStunned()) return true;
            return defaultIsDisabled();
        };
    }

    protected override GameObject LoadPrefab(GameObject parent)
    {
        Character character = parent.GetComponent<Character>();

        if (character != null)
        {
            string prefabPath = character.Side == PlayerType.blue ? blueStunPrefabPath : pinkStunPrefabPath;
            return Resources.Load<GameObject>(prefabPath);
        }

        return null;
    }
}
