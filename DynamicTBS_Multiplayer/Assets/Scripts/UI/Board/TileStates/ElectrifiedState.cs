using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifiedState : State
{
    protected override int Duration { get { return 2; } }
    private static readonly string spritePath = "EffectSprites/ElectrifiedTile";

    private Tile tile;
    private Character stunnedInhabitant;

    public ElectrifiedState(GameObject parent) : base(parent)
    {
        tile = Board.GetTileByPosition(parent.transform.position);

        StunInhabitant();
        GameplayEvents.OnFinishAction += StunInhabitant;
    }

    protected override Sprite LoadSprite(GameObject parent)
    {
        return Resources.Load<Sprite>(spritePath);
    }

    private void StunInhabitant(ActionMetadata actionMetadata)
    {
        if(tile.IsOccupied() && tile.GetCurrentInhabitant() != stunnedInhabitant)
        {
            StunInhabitant();
        }
    }

    private void StunInhabitant()
    {
        if (tile.IsOccupied())
        {
            stunnedInhabitant = tile.GetCurrentInhabitant();
            stunnedInhabitant.SetState(CharacterStateType.STUNNED);
            currentCount = 2*StunnedState.StunDuration + 1;
        }
    }

    public override void Destroy()
    {
        GameplayEvents.OnFinishAction -= StunInhabitant;
        base.Destroy();
    }

    ~ElectrifiedState()
    {
        Destroy();
    }
}
