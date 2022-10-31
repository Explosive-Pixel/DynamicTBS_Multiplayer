using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAAHandler : MonoBehaviour
{
    List<BlockAA> activeBlocks = new List<BlockAA>();

    private void Awake()
    {
        SubscribeEvents();
    }
    public void ExecuteBlockAA(BlockAA blockAA)
    {
        blockAA.ActivateBlock();
        activeBlocks.Add(blockAA);
        GameplayEvents.ActionFinished(UIActionType.ActiveAbility_Block);
    }

    private void ReduceBlockCounters(Player player)
    {
        List<BlockAA> finishedBlocks = new List<BlockAA>();

        foreach (BlockAA blockAA in activeBlocks)
        {
            if(blockAA.GetCharacter().GetSide() == player)
            {
                blockAA.ReduceBlockCount();

                if (!blockAA.IsBlocking())
                {
                    finishedBlocks.Add(blockAA);
                }
            }
            
        }

        activeBlocks.RemoveAll(blockAA => finishedBlocks.Contains(blockAA));
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded += ReduceBlockCounters;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded -= ReduceBlockCounters;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
