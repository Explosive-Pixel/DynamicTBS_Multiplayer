using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class IndicatorEffect : MonoBehaviour
{
    protected void AnimateIndicator(GameObject prefab, Vector3 position, float indicatorTime)
    {
        GameObject indicatorGameObject = Instantiate(prefab, position, Quaternion.identity);
        indicatorGameObject.transform.SetParent(Board.GetTileByPosition(position).transform);
        StartCoroutine(ExplosionCoroutine(indicatorGameObject, indicatorTime));
    }


    protected IEnumerator ExplosionCoroutine(GameObject indicatorGameObject, float timerDuration)
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
