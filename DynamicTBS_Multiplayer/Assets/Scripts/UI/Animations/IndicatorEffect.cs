using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class IndicatorEffect : MonoBehaviour
{
    protected void AnimateIndicator(GameObject prefab, Vector3 position, float indicatorTime)
    {
        if (GameplayManager.IsLoadingGame)
            return;

        Tile effectTile = Board.GetTileByPosition(position);
        GameObject indicatorGameObject = Instantiate(prefab);
        indicatorGameObject.transform.SetParent(effectTile.transform, false);
        indicatorGameObject.transform.position = effectTile.transform.position;
        StartCoroutine(EffectCoroutine(indicatorGameObject, indicatorTime));
    }


    protected IEnumerator EffectCoroutine(GameObject indicatorGameObject, float timerDuration)
    {
        float timerEndpoint = 0;

        while (timerDuration > timerEndpoint)
        {
            timerDuration -= Time.deltaTime;
            yield return null;
        }

        Destroy(indicatorGameObject);
        yield break;
    }

    protected GameObject GetRandomPrefab(List<GameObject> prefabs)
    {
        return prefabs.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
    }
}
