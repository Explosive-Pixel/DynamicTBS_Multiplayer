using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : MonoBehaviour, IState
{
    private Character character;

    public static StunnedState Create(GameObject parent, int stunDuration)
    {
        StunnedState ss = parent.AddComponent<StunnedState>();
        ss.Init(stunDuration);

        return ss;
    }

    private void Init(int stunDuration)
    {
        character = gameObject.GetComponent<Character>();

        var defaultIsDisabled = character.isDisabled;
        character.isDisabled = () =>
        {
            if (IsStunned(character)) return true;
            return defaultIsDisabled();
        };

        VisualizeStun(true);

        RoundBasedCounter.Create(gameObject, stunDuration, Destroy);
    }

    private void VisualizeStun(bool active)
    {
        gameObject.GetComponentInChildren<StunHandler>(true).VisualizeStun(active);
    }

    private bool IsStunned(Character character)
    {
        return character.gameObject.GetComponent<StunnedState>() != null;
    }

    public void Destroy()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        VisualizeStun(false);
    }
}
