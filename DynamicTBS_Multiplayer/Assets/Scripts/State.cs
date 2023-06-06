using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected abstract int Duration { get; }
    protected abstract Sprite Sprite { get; }

    protected int currentCount = 0;
    protected GameObject overlay;

    public State(GameObject parent)
    {
        this.currentCount = Duration*2 + 1;

        if(this.currentCount > 0)
        {
            ShowSprite(parent);
            GameplayEvents.OnPlayerTurnEnded += ReduceCurrentCount;
        }
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

    private void ShowSprite(GameObject parent)
    {
        if (Sprite != null)
        {
            overlay = new GameObject();
            Quaternion startRotation = Quaternion.identity;
            SpriteRenderer spriteRenderer = overlay.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite;
            spriteRenderer.sortingOrder = parent.GetComponent<SpriteRenderer>().sortingOrder;
            overlay.transform.position = parent.transform.position;
            overlay.transform.rotation = startRotation;
            overlay.transform.SetParent(parent.transform);
        }
    }

    ~State()
    {
        Destroy();
    }
}
