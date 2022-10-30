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
    private PrefabProvider prefabProvider;

    private void Awake()
    {
        SubscribeEvents();
        prefabProvider = GameObject.Find("PrefabProvider").GetComponent<PrefabProvider>();
    }
    
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        /*if (tmpGameObjectsByUIActionType.Count == 0 && !activeAbilityCurrentlyPerformed)
        {
            UIEvents.InformNoActionDestinationAvailable();
            return;
        }*/
        
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

    private void InstantiateActionPositions(List<Vector3> positions, UIActionType type)
    {
        if (positions.Count > 0)
        {
            ResetTmpList(type);

            List<GameObject> gameObjects = new List<GameObject>();
            GameObject prefab = prefabProvider.GetActionPrefabByActionType(type);
            foreach (Vector3 position in positions)
            {
                prefab.transform.position = new Vector3(position.x, position.y, 0.98f);
                gameObjects.Add(Instantiate(prefab));
            }

            if (gameObjects.Count > 0)
                tmpGameObjectsByUIActionType.Add(type, gameObjects);
        }
    }

    private void ActionOver(UIActionType type) 
    {
        ResetTmpList();
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

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        UIEvents.OnPassActionPositionsList += InstantiateActionPositions;
        GameplayEvents.OnFinishAction += ActionOver;
        GameplayEvents.OnExecuteActiveAbility += ResetTmpList;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassActionPositionsList -= InstantiateActionPositions;
        GameplayEvents.OnFinishAction -= ActionOver;
        GameplayEvents.OnExecuteActiveAbility -= ResetTmpList;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
