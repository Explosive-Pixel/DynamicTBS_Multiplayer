using UnityEngine;

public class PrefabProvider : MonoBehaviour
{
    public GameObject moveCirclePrefab;
    public GameObject attackCirclePrefab;
    public GameObject activeAbilityCirclePrefab;

    public GameObject GetActionPrefabByActionType(UIActionType type) 
    {
        switch (type)
        {
            case UIActionType.Move:
                {
                    return moveCirclePrefab;
                }
            case UIActionType.Attack:
                {
                    return attackCirclePrefab;
                }
            case UIActionType.ActiveAbility:
                {
                    return activeAbilityCirclePrefab;
                }
        }

        return null;
    }
}
