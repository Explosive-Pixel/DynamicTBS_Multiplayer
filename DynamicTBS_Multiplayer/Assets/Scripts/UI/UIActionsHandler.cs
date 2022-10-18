using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject moveCirclePrefab;
    [SerializeField] private GameObject attackCirclePrefab;
    private List<GameObject> tmpGameObjects = new List<GameObject>();
    private Camera currentCamera;
    private bool isPlacing;

    private void Awake()
    {
        SubscribeEvents();
        isPlacing = true;
    }
    
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if (isPlacing) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        GameObject uiObject = GetUIObjectByClickPosition(Input.mousePosition);

        if (uiObject == null) return;
        
        UIEvents.PassMoveDestination(uiObject.transform.position);
        ResetTmpList();
    }

    private GameObject GetUIObjectByClickPosition(Vector3 position) 
    {
        Ray ray = currentCamera.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits != null && hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (tmpGameObjects.Contains(gameObject))
                    return gameObject;
            }
        }

        return null;
    }

    private void ResetTmpList()
    {
        foreach (GameObject gameObject in tmpGameObjects)
        {
            GameObject.Destroy(gameObject);
        }
        
        tmpGameObjects.Clear();
        isPlacing = true;
    }

    private void InstantiateMovePositions(List<Vector3> positions)
    {
        if (positions.Count > 0)
        {
            Debug.Log("InstantiatingMovePositions");
            ResetTmpList();
            foreach (Vector3 position in positions)
            {
                moveCirclePrefab.transform.position = new Vector3(position.x, position.y, 0.98f);
                tmpGameObjects.Add(Instantiate(moveCirclePrefab));
            }
            isPlacing = false;
        }
        else 
        {
            UIEvents.InformNoMoveDestinationAvailable();
        }
    }

    private void InstantiateAttackPositions(List<Vector3> positions)
    {
        if (positions.Count > 0)
        {
            Debug.Log("InstantiatingAttackPositions");
            ResetTmpList();
            foreach (Vector3 position in positions)
            {
                attackCirclePrefab.transform.position = new Vector3(position.x, position.y, 0.98f);
                tmpGameObjects.Add(Instantiate(attackCirclePrefab));
            }
            isPlacing = false;
        }
        else
        {
            UIEvents.InformNoMoveDestinationAvailable();
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        UIEvents.OnPassMovePositionsList += InstantiateMovePositions;
        UIEvents.OnPassAttackPositionsList += InstantiateAttackPositions;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassMovePositionsList -= InstantiateMovePositions;
        UIEvents.OnPassAttackPositionsList -= InstantiateAttackPositions;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
