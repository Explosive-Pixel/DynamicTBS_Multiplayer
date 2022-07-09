using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject moveCirclePrefab;
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
        GameObject uiObject = GetUIObjectByPosition(Input.mousePosition);

        if (uiObject == null) return;
        
        UIEvents.PassMoveDestination(uiObject.transform.position);
        ResetTmpList();
    }

    private GameObject GetUIObjectByPosition(Vector3 position) 
    {
        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject gameObject = hit.transform.gameObject;
            if (tmpGameObjects.Contains(gameObject))
                return gameObject;
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
        ResetTmpList();
        foreach (Vector3 position in positions)
        {
            moveCirclePrefab.transform.position = new Vector3(position.x, position.y, 0.98f);
            tmpGameObjects.Add(Instantiate(moveCirclePrefab));
        }
        isPlacing = false;
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
