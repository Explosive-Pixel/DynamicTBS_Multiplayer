using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected abstract int Duration { get; }

    protected int currentCount = 0;
    protected GameObject overlay;

    public State(GameObject parent)
    {
        this.currentCount = Duration*2 + 1;

        if(this.currentCount > 0)
        {
            ShowPrefab(parent);
            GameplayEvents.OnPlayerTurnEnded += ReduceCurrentCount;
        }
    }

    protected virtual GameObject LoadPrefab(GameObject parent)
    {
        return null;
    }

    private void ReduceCurrentCount(Player player)
    {
        currentCount--;

        if(currentCount == 0)
        {
            Destroy();
        }
    }

    public bool IsActive()
    {
        return currentCount > 0;
    }

    public virtual void Destroy()
    {
        if (overlay != null)
            GameObject.Destroy(overlay);

        GameplayEvents.OnPlayerTurnEnded -= ReduceCurrentCount;
    }

    private void ShowPrefab(GameObject parent)
    {
        GameObject statePrefab = LoadPrefab(parent);

        if (statePrefab != null)
        {
            GameObject state = GameObject.Instantiate(statePrefab);
            state.transform.position = parent.transform.position;
            state.SetActive(true);
            state.transform.SetParent(parent.transform);
        }
    }

    ~State()
    {
        Destroy();
    }
}
