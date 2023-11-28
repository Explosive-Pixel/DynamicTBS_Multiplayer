using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundBasedCounter : MonoBehaviour
{
    public delegate void ExpireFunction();

    public int duration;
    public ExpireFunction expireFunction;

    private int currentCount;

    public static RoundBasedCounter Create(GameObject parent, int duration, ExpireFunction expireFunction)
    {
        RoundBasedCounter rbc = parent.AddComponent<RoundBasedCounter>();
        rbc.Init(duration, expireFunction);

        return rbc;
    }

    private void Init(int duration, ExpireFunction expireFunction)
    {
        this.duration = duration;
        this.expireFunction = expireFunction;

        currentCount = duration * 2 + 1;

        if (currentCount > 0)
        {
            GameplayEvents.OnPlayerTurnEnded += ReduceCurrentCount;
        }
    }

    private void ReduceCurrentCount(PlayerType player)
    {
        currentCount--;

        if (currentCount == 0)
        {
            OnExpiration();
        }
    }

    public void OnExpiration()
    {
        expireFunction();
        GameplayEvents.OnPlayerTurnEnded -= ReduceCurrentCount;
        Destroy(this);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnPlayerTurnEnded -= ReduceCurrentCount;
    }
}
