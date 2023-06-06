using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : State
{
    public const int StunDuration = 1;

    protected override int Duration { get { return StunDuration; } }
    protected override Sprite Sprite { get { return null; } }

    public StunnedState(GameObject parent) : base(parent)
    {
    }
}
