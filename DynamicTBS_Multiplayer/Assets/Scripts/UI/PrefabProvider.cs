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
            case UIActionType.ActiveAbility_Heal:
                {
                    return activeAbilityCirclePrefab;
                }
            case UIActionType.ActiveAbility_Jump:
                {
                    return moveCirclePrefab;
                }
            case UIActionType.ActiveAbility_ChangeFloor:
                {
                    return activeAbilityCirclePrefab;
                }
        }

        return null;
    }
}
