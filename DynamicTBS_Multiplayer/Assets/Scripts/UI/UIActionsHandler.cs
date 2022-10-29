using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionsHandler : MonoBehaviour
{
    [SerializeField] private GameObject moveCirclePrefab;
    [SerializeField] private GameObject attackCirclePrefab;
    private Dictionary<UIActionType, List<GameObject>> tmpGameObjectsByUIActionType = new Dictionary<UIActionType, List<GameObject>>();
    private Camera currentCamera;

    private void Awake()
    {
        SubscribeEvents();
    }
    
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if (tmpGameObjectsByUIActionType.Count == 0)
        {
            UIEvents.InformNoActionDestinationAvailable();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        (GameObject uiObject, UIActionType? actionType) = GetUIObjectByClickPosition(Input.mousePosition);

        if (uiObject == null || actionType == null) return;

        UIEvents.PassActionDestination(uiObject.transform.position, (UIActionType) actionType);
        ResetTmpList();
    }

    private (GameObject, UIActionType?) GetUIObjectByClickPosition(Vector3 position) 
    {
        Ray ray = currentCamera.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits != null && hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                GameObject gameObject = hit.transform.gameObject;
                var actionType = IsUIObject(gameObject);
                if (actionType != null)
                    return (gameObject, actionType);
            }
        }

        return (null, null);
    }

    private UIActionType? IsUIObject(GameObject gameObject)
    {
        foreach (var tmpGameObjects in tmpGameObjectsByUIActionType)
        {
            if (tmpGameObjects.Value.Contains(gameObject))
            {
                return tmpGameObjects.Key;
            }
        }
        return null;
    }

    private void ResetTmpList() 
    {
        List<UIActionType> keys = new List<UIActionType>(tmpGameObjectsByUIActionType.Keys);
        foreach (UIActionType type in keys)
        {
            ResetTmpList(type);
        }
    }

    private void ResetTmpList(UIActionType type)
    {
        if (!tmpGameObjectsByUIActionType.ContainsKey(type)) 
        {
            return;
        }

        foreach (GameObject gameObject in tmpGameObjectsByUIActionType.GetValueOrDefault(type))
        {
            GameObject.Destroy(gameObject);
        }

        tmpGameObjectsByUIActionType.Remove(type);
    }

    private void InstantiateActionPositions(List<Vector3> positions, UIActionType type)
    {
        switch (type)
        {
            case UIActionType.Move:
            {
                InstantiateMovePositions(positions);
                break;
            }
            case UIActionType.Attack:
            {
                InstantiateAttackPositions(positions);
                break;
            }
        }
    }

    private void InstantiateMovePositions(List<Vector3> positions)
    {
        if (positions.Count > 0)
        {
            ResetTmpList(UIActionType.Move);
            List<GameObject> moveGameObjects = new List<GameObject>();
            foreach (Vector3 position in positions)
            {
                moveCirclePrefab.transform.position = new Vector3(position.x, position.y, 0.98f);
                moveGameObjects.Add(Instantiate(moveCirclePrefab));
            }

            if(moveGameObjects.Count > 0)
                tmpGameObjectsByUIActionType.Add(UIActionType.Move, moveGameObjects);
        }
    }

    private void InstantiateAttackPositions(List<Vector3> positions)
    {
        if (positions.Count > 0)
        {
            ResetTmpList(UIActionType.Attack);
            List<GameObject> attackGameObjects = new List<GameObject>();
            foreach (Vector3 position in positions)
            {
                attackCirclePrefab.transform.position = new Vector3(position.x, position.y, 0.98f);
                attackGameObjects.Add(Instantiate(attackCirclePrefab));
            }

            if(attackGameObjects.Count > 0)
                tmpGameObjectsByUIActionType.Add(UIActionType.Attack, attackGameObjects);
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        UIEvents.OnPassActionPositionsList += InstantiateActionPositions;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassActionPositionsList -= InstantiateActionPositions;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
