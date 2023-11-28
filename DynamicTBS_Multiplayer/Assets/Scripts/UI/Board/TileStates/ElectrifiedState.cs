using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifiedState : MonoBehaviour, IState
{
    private GameObject electrifyPrefab;
    private int stunDuration;

    private Tile tile;
    private Character stunnedInhabitant;
    private GameObject overlay;

    public static ElectrifiedState Create(GameObject parent, int duration, int stunDuration, GameObject electrifyPrefab)
    {
        ElectrifiedState es = parent.AddComponent<ElectrifiedState>();
        es.Init(duration, stunDuration, electrifyPrefab);

        return es;
    }

    private void Init(int duration, int stunDuration, GameObject electrifyPrefab)
    {
        this.stunDuration = stunDuration;
        this.electrifyPrefab = electrifyPrefab;

        ElectrifyTile();

        RoundBasedCounter.Create(gameObject, duration, Destroy);
    }

    private void ElectrifyTile()
    {
        tile = Board.GetTileByPosition(gameObject.transform.position);

        CreateOverlay();

        GameplayEvents.OnFinishAction += StunInhabitant;
        StunInhabitant();
    }

    private void CreateOverlay()
    {
        overlay = Instantiate(electrifyPrefab);
        overlay.transform.position = gameObject.transform.position;
        overlay.SetActive(true);
        overlay.transform.SetParent(gameObject.transform);
    }

    private void StunInhabitant(ActionMetadata actionMetadata)
    {
        if (tile.IsOccupied() && tile.CurrentInhabitant != stunnedInhabitant)
        {
            StunInhabitant();
        }
    }

    private void StunInhabitant()
    {
        if (tile.IsOccupied())
        {
            stunnedInhabitant = tile.CurrentInhabitant;
            StunnedState.Create(stunnedInhabitant.gameObject, stunDuration);
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        Destroy(overlay);
        stunnedInhabitant = null;
        GameplayEvents.OnFinishAction -= StunInhabitant;
    }
}
