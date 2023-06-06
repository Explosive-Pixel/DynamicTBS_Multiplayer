using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifiedState : State
{
    protected override int Duration { get { return 2; } }
    protected override Sprite Sprite { get { return TileSpriteManager.BLUE_MASTER_START_TILE_SPRITE; } }

    private Tile tile;
    private Character stunnedInhabitant;

    public ElectrifiedState(GameObject parent) : base(parent)
    {
        tile = Board.GetTileByPosition(parent.transform.position);

        StunInhabitant();
        GameplayEvents.OnFinishAction += StunInhabitant;
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
