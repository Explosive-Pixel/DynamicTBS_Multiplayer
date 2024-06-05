using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : MonoBehaviour, IState
{
    private Character character;

    public static StunnedState Create(GameObject parent, int stunDuration, PlayerType stunningSide)
    {
        StunnedState ss = parent.AddComponent<StunnedState>();
        ss.Init(stunDuration, stunningSide);

        return ss;
    }

    private void Init(int stunDuration, PlayerType stunningSide)
    {
        character = gameObject.GetComponent<Character>();

        var defaultIsDisabled = character.isDisabled;
        character.isDisabled = () =>
        {
            if (IsStunned(character)) return true;
            return defaultIsDisabled();
        };

        VisualizeStun(stunningSide);

        RoundBasedCounter.Create(gameObject, stunDuration, Destroy);
    }

    private void VisualizeStun(PlayerType stunningSide)
    {
        gameObject.GetComponentInChildren<StunHandler>(true).VisualizeStun(stunningSide);
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
        gameObject.GetComponentInChildren<StunHandler>().TurnOffStunVisualization();
    }
}
