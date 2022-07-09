using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject moveCirclePrefab;
    private List<GameObject> tmpGameObjects = new List<GameObject>();

    private void Awake()
    {
        SubscribeEvents();
    }

    private void ResetTmpList()
    {
        foreach (GameObject gameObject in tmpGameObjects)
        {
            GameObject.Destroy(gameObject);
        }
        
        tmpGameObjects.Clear();
    }

    private void InstantiateMovePositions(List<Vector3> positions)
    {
        ResetTmpList();
        foreach (Vector3 position in positions)
        {
            moveCirclePrefab.transform.position = new Vector3(position.x, position.y, 0.98f);
            tmpGameObjects.Add(Instantiate(moveCirclePrefab));
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        UIEvents.OnPassMovePositionsList += InstantiateMovePositions;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassMovePositionsList -= InstantiateMovePositions;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
